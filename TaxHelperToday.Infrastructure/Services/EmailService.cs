using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ApplicationDbContext context, ILogger<EmailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, string? fromName = null)
    {
        try
        {
            var smtpSettings = await GetSmtpSettingsAsync();
            
            if (!smtpSettings.IsEnabled)
            {
                _logger.LogWarning("SMTP is disabled. Email not sent to {To}", to);
                return false;
            }

            if (string.IsNullOrWhiteSpace(smtpSettings.Host) || string.IsNullOrWhiteSpace(smtpSettings.FromEmail))
            {
                _logger.LogError("SMTP settings are incomplete. Cannot send email.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(smtpSettings.Username) || string.IsNullOrWhiteSpace(smtpSettings.Password))
            {
                _logger.LogError("SMTP username or password is missing. Cannot send email.");
                return false;
            }

            using var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port);
            client.EnableSsl = smtpSettings.EnableSsl;
            client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 30000; // 30 seconds

            var siteName = await GetSiteNameAsync();
            var fromAddress = new MailAddress(smtpSettings.FromEmail, fromName ?? smtpSettings.FromName ?? siteName);
            var toAddress = new MailAddress(to);

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            await client.SendMailAsync(message);
            
            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}: {Message}", to, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendEnquiryNotificationAsync(string enquiryType, string name, string email, string? phone, string subject, string message, string? serviceName = null)
    {
        try
        {
            var smtpSettings = await GetSmtpSettingsAsync();
            
            if (!smtpSettings.IsEnabled)
            {
                _logger.LogWarning("SMTP is disabled. Enquiry notification not sent.");
                return false;
            }

            // Get admin notification email from settings
            var adminEmail = await _context.AdminSettings
                .FirstOrDefaultAsync(s => s.Key == "smtp_admin_notification_email");
            
            var notificationEmail = adminEmail?.Value ?? smtpSettings.FromEmail;
            var siteName = await GetSiteNameAsync();

            var emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .field {{ margin-bottom: 15px; }}
        .label {{ font-weight: bold; color: #555; }}
        .value {{ color: #333; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>New {enquiryType}</h2>
        </div>
        <div class=""content"">
            <div class=""field"">
                <span class=""label"">Name:</span>
                <span class=""value"">{name}</span>
            </div>
            <div class=""field"">
                <span class=""label"">Email:</span>
                <span class=""value""><a href=""mailto:{email}"">{email}</a></span>
            </div>
            {(string.IsNullOrWhiteSpace(phone) ? "" : $@"<div class=""field"">
                <span class=""label"">Phone:</span>
                <span class=""value""><a href=""tel:{phone}"">{phone}</a></span>
            </div>")}
            {(string.IsNullOrWhiteSpace(serviceName) ? "" : $@"<div class=""field"">
                <span class=""label"">Service:</span>
                <span class=""value"">{serviceName}</span>
            </div>")}
            <div class=""field"">
                <span class=""label"">Subject:</span>
                <span class=""value"">{subject}</span>
            </div>
            <div class=""field"">
                <span class=""label"">Message:</span>
                <div class=""value"" style=""margin-top: 10px; padding: 10px; background-color: white; border: 1px solid #ddd; border-radius: 4px;"">
                    {message.Replace("\n", "<br>")}
                </div>
            </div>
        </div>
        <div class=""footer"">
            <p>This is an automated notification from {siteName}.</p>
            <p>Please respond to the customer at <a href=""mailto:{email}"">{email}</a></p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(
                notificationEmail,
                $"New {enquiryType}: {subject}",
                emailBody,
                isHtml: true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending enquiry notification: {Message}", ex.Message);
            return false;
        }
    }

    public async Task<bool> SendEnquiryConfirmationAsync(string to, string name, string enquiryType)
    {
        try
        {
            var smtpSettings = await GetSmtpSettingsAsync();
            
            if (!smtpSettings.IsEnabled)
            {
                _logger.LogWarning("SMTP is disabled. Confirmation email not sent to {To}", to);
                return false;
            }

            var siteName = await GetSiteNameAsync();

            var emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>Thank You for Contacting Us!</h2>
        </div>
        <div class=""content"">
            <p>Dear {name},</p>
            <p>Thank you for your {enquiryType.ToLower()}. We have received your message and our team will get back to you within one business day.</p>
            <p>We appreciate your interest in {siteName} and look forward to assisting you with your tax and compliance needs.</p>
            <p>If you have any urgent questions, please feel free to contact us directly.</p>
            <p>Best regards,<br>The {siteName} Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated confirmation email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(
                to,
                $"Thank you for your {enquiryType} - {siteName}",
                emailBody,
                isHtml: true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email to {To}: {Message}", to, ex.Message);
            return false;
        }
    }

    private async Task<SmtpSettings> GetSmtpSettingsAsync()
    {
        var settings = await _context.AdminSettings
            .Where(s => s.Key.StartsWith("smtp_"))
            .ToListAsync();

        var settingsDict = settings.ToDictionary(s => s.Key, s => s.Value ?? string.Empty);
        var siteName = await GetSiteNameAsync();

        return new SmtpSettings
        {
            IsEnabled = settingsDict.GetValueOrDefault("smtp_enabled", "false").Equals("true", StringComparison.OrdinalIgnoreCase),
            Host = settingsDict.GetValueOrDefault("smtp_host", string.Empty),
            Port = int.TryParse(settingsDict.GetValueOrDefault("smtp_port", "587"), out var port) ? port : 587,
            Username = settingsDict.GetValueOrDefault("smtp_username", string.Empty),
            Password = settingsDict.GetValueOrDefault("smtp_password", string.Empty),
            FromEmail = settingsDict.GetValueOrDefault("smtp_from_email", string.Empty),
            FromName = settingsDict.GetValueOrDefault("smtp_from_name", siteName),
            EnableSsl = settingsDict.GetValueOrDefault("smtp_enable_ssl", "true").Equals("true", StringComparison.OrdinalIgnoreCase)
        };
    }

    private async Task<string> GetSiteNameAsync()
    {
        var siteNameSetting = await _context.AdminSettings
            .FirstOrDefaultAsync(s => s.Key == "site_name");
        return siteNameSetting?.Value ?? "TaxHelperToday";
    }

    private class SmtpSettings
    {
        public bool IsEnabled { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
