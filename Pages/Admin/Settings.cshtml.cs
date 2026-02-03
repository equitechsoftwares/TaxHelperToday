using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Admin.Domain.Entities;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin
{
    public class SettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SettingsModel> _logger;

        public SettingsModel(
            ApplicationDbContext context,
            IIdentityService identityService,
            IEmailService emailService,
            ILogger<SettingsModel> logger)
        {
            _context = context;
            _identityService = identityService;
            _emailService = emailService;
            _logger = logger;
        }

        public List<AdminSetting> Settings { get; set; } = new();

        [BindProperty]
        public Dictionary<string, string> SettingValues { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Ensure default settings exist (idempotent)
            await SeedDefaultSettingsAsync();

            // Group settings by category for better organization
            Settings = await _context.AdminSettings
                .OrderBy(s => s.Key.StartsWith("contact_") ? 0 : s.Key.StartsWith("smtp_") ? 1 : s.Key.StartsWith("site_") ? 2 : 3)
                .ThenBy(s => s.Key)
                .ToListAsync();

            // Initialize SettingValues with current values
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
                // Contact Information
                new AdminSetting { Key = "contact_email", Value = "mrhoque64@gmail.com", Description = "Contact Email" },
                new AdminSetting { Key = "contact_phone", Value = "+91-89103-97497", Description = "Contact Phone" },
                new AdminSetting { Key = "contact_working_hours", Value = "Monday to Saturday, 10:00 AM – 7:00 PM IST", Description = "Working Hours" },
                new AdminSetting { Key = "contact_whatsapp_number", Value = "918910397497", Description = "WhatsApp Number (without + or spaces)" },
                new AdminSetting { Key = "contact_whatsapp_message", Value = "Hi, I'd like to get in touch with TaxHelperToday", Description = "WhatsApp Pre-filled Message" },
                new AdminSetting { Key = "contact_office_address", Value = "TaxHelperToday\n1, Royd Ln, Esplanade\nTaltala, Kolkata, West Bengal - 700016\nIndia", Description = "Office Address" },
                new AdminSetting { Key = "contact_map_url", Value = "https://www.google.com/maps?q=1+Royd+Ln,+Esplanade,+Taltala,+Kolkata,+West+Bengal+700016&output=embed", Description = "Google Maps Embed URL" },
                new AdminSetting { Key = "contact_map_directions_url", Value = "https://www.google.com/maps/place/1,+Royd+Ln,+Esplanade,+Taltala,+Kolkata,+West+Bengal+700016/@@22.5517171,88.3537581,17z/data=!3m1!4b1!4m5!3m4!1s0x3a02771b30ffd405:0x30a20c3cf4c869fd!8m2!3d22.5517122!4d88.356333?entry=ttu", Description = "Google Maps Directions URL" },

                // SMTP Email Settings
                new AdminSetting { Key = "smtp_enabled", Value = "false", Description = "Enable Email Sending" },
                new AdminSetting { Key = "smtp_host", Value = "smtp.gmail.com", Description = "SMTP Server Host" },
                new AdminSetting { Key = "smtp_port", Value = "587", Description = "SMTP Port (587 for TLS, 465 for SSL)" },
                new AdminSetting { Key = "smtp_username", Value = "", Description = "SMTP Username/Email" },
                new AdminSetting { Key = "smtp_password", Value = "", Description = "SMTP Password (App Password for Gmail)" },
                new AdminSetting { Key = "smtp_from_email", Value = "mrhoque64@gmail.com", Description = "From Email Address" },
                new AdminSetting { Key = "smtp_from_name", Value = "TaxHelperToday", Description = "From Name" },
                new AdminSetting { Key = "smtp_enable_ssl", Value = "true", Description = "Enable SSL/TLS" },
                new AdminSetting { Key = "smtp_admin_notification_email", Value = "mrhoque64@gmail.com", Description = "Admin Notification Email (where enquiry notifications are sent)" },

                // Site Settings
                new AdminSetting { Key = "site_name", Value = "TaxHelperToday", Description = "Site Name" },
                new AdminSetting { Key = "it_deadline_countdown", Value = "true", Description = "Show ITR deadline countdown" },
                new AdminSetting { Key = "it_deadline_date", Value = "2024-07-31", Description = "ITR filing deadline date" },
                new AdminSetting { Key = "it_deadline_title", Value = "ITR Filing Deadline Approaching!", Description = "ITR deadline banner title" },
                new AdminSetting { Key = "it_deadline_text", Value = "File now to avoid penalties and maximize your refund.", Description = "ITR deadline banner description (after the days counter)." },
                new AdminSetting { Key = "it_deadline_button_text", Value = "File ITR Now", Description = "ITR deadline banner button text" },
                new AdminSetting { Key = "it_deadline_support_template", Value = "Need help? Call {{phone}} or email {{email}}.", Description = "ITR deadline banner support line. Use {{phone}} and {{email}} placeholders." }
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
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }

            try
            {
                var settings = await _context.AdminSettings.ToListAsync();

                foreach (var kvp in SettingValues)
                {
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
                        // TODO: Set UpdatedBy from current user
                    }
                    else
                    {
                        // Create new setting if it doesn't exist
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

                TempData["SuccessMessage"] = "Settings updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating settings: {ex.Message}");
                Settings = await _context.AdminSettings
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLogoutAllSessionsAsync()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    TempData["ErrorMessage"] = "Unable to identify user.";
                    return RedirectToPage();
                }

                var revokedCount = await _identityService.RevokeAllRefreshTokensAsync(userId);
                
                _logger.LogInformation("User {UserId} revoked {Count} active sessions", userId, revokedCount);
                
                TempData["SuccessMessage"] = $"Successfully logged out from {revokedCount} active session(s). You will need to log in again on other devices.";
                
                // Also clear current session cookies
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1)
                };
                
                Response.Cookies.Append("access_token", string.Empty, cookieOptions);
                Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);
                
                return RedirectToPage("/Admin/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all sessions for user");
                TempData["ErrorMessage"] = "An error occurred while revoking sessions. Please try again.";
                return RedirectToPage();
            }
        }

        [BindProperty]
        public string? TestEmailAddress { get; set; }

        public async Task<IActionResult> OnPostTestEmailAsync(string? testEmailAddress)
        {
            try
            {
                // Get admin notification email or use provided email
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

                // Validate email format
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
