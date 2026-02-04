using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Pages.Admin
{
    public class EmailSettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailSettingsModel> _logger;

        public EmailSettingsModel(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<EmailSettingsModel> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public List<AdminSetting> Settings { get; set; } = new();

        [BindProperty]
        public Dictionary<string, string> SettingValues { get; set; } = new();

        [BindProperty]
        public string? TestEmailAddress { get; set; }

        public async Task OnGetAsync()
        {
            await SeedDefaultSettingsAsync();

            Settings = await _context.AdminSettings
                .Where(s => s.Key.StartsWith("smtp_"))
                .OrderBy(s => s.Key)
                .ToListAsync();

            foreach (var setting in Settings)
            {
                SettingValues[setting.Key] = setting.Value ?? string.Empty;
            }
        }

        private async Task SeedDefaultSettingsAsync()
        {
            var existingSettings = await _context.AdminSettings.ToListAsync();
            var existingKeys = new HashSet<string>(existingSettings.Select(s => s.Key));

            var defaultSettings = new[]
            {
                new AdminSetting { Key = "smtp_enabled", Value = "false", Description = "Enable Email Sending" },
                new AdminSetting { Key = "smtp_host", Value = "smtpout.secureserver.net", Description = "SMTP Server Host" },
                new AdminSetting { Key = "smtp_port", Value = "25", Description = "SMTP Port" },
                new AdminSetting { Key = "smtp_username", Value = "support@taxhelpertoday.com", Description = "SMTP Username/Email" },
                new AdminSetting { Key = "smtp_password", Value = "Falc0n!Gold3n$", Description = "SMTP Password" },
                new AdminSetting { Key = "smtp_from_email", Value = "support@taxhelpertoday.com", Description = "From Email Address" },
                new AdminSetting { Key = "smtp_from_name", Value = "TaxHelperToday", Description = "From Name" },
                new AdminSetting { Key = "smtp_enable_ssl", Value = "false", Description = "Enable SSL/TLS" },
                new AdminSetting { Key = "smtp_admin_notification_email", Value = "support@taxhelpertoday.com", Description = "Admin Notification Email (where enquiry notifications are sent)" }
            };

            var newSettings = defaultSettings
                .Where(s => !existingKeys.Contains(s.Key))
                .ToList();

            if (newSettings.Any())
            {
                _context.AdminSettings.AddRange(newSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("smtp_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }

            try
            {
                var settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("smtp_"))
                    .ToListAsync();

                foreach (var kvp in SettingValues)
                {
                    if (!kvp.Key.StartsWith("smtp_"))
                        continue;

                    var setting = settings.FirstOrDefault(s => s.Key == kvp.Key);
                    if (setting != null)
                    {
                        // For password fields, only update if a new value is provided
                        if (kvp.Key == "smtp_password" && string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            // Keep existing password, don't update
                            continue;
                        }
                        
                        setting.Value = kvp.Value;
                        setting.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        var newSetting = new AdminSetting
                        {
                            Key = kvp.Key,
                            Value = kvp.Value,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.AdminSettings.Add(newSetting);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Email settings updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email settings");
                ModelState.AddModelError(string.Empty, $"Error updating settings: {ex.Message}");
                Settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("smtp_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostTestEmailAsync(string? testEmailAddress)
        {
            try
            {
                var adminEmailSetting = await _context.AdminSettings
                    .FirstOrDefaultAsync(s => s.Key == "smtp_admin_notification_email");
                
                var testEmail = !string.IsNullOrWhiteSpace(testEmailAddress) 
                    ? testEmailAddress.Trim() 
                    : adminEmailSetting?.Value;

                if (string.IsNullOrWhiteSpace(testEmail))
                {
                    TempData["ErrorMessage"] = "Please provide an email address to send the test email to, or configure the Admin Notification Email setting.";
                    return RedirectToPage();
                }

                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(testEmail);
                }
                catch
                {
                    TempData["ErrorMessage"] = "Invalid email address format.";
                    return RedirectToPage();
                }

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
            <h2>Test Email from TaxHelperToday</h2>
        </div>
        <div class=""content"">
            <p>Congratulations! Your SMTP email settings are configured correctly.</p>
            <p>This is a test email sent from the TaxHelperToday admin panel to verify that email functionality is working properly.</p>
            <p><strong>Test Details:</strong></p>
            <ul>
                <li>Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</li>
                <li>Recipient: {testEmail}</li>
                <li>Status: Email delivery successful</li>
            </ul>
            <p>If you received this email, your SMTP configuration is correct and enquiry notifications will be sent automatically.</p>
        </div>
        <div class=""footer"">
            <p>This is an automated test email from TaxHelperToday.</p>
        </div>
    </div>
</body>
</html>";

                var success = await _emailService.SendEmailAsync(
                    testEmail,
                    "Test Email - TaxHelperToday SMTP Configuration",
                    emailBody,
                    isHtml: true
                );

                if (success)
                {
                    TempData["SuccessMessage"] = $"Test email sent successfully to {testEmail}. Please check your inbox (and spam folder).";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send test email. Please check your SMTP settings and ensure 'Enable Email Sending' is checked. Check the application logs for more details.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                TempData["ErrorMessage"] = $"An error occurred while sending the test email: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}
