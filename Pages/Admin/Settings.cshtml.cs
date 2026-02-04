using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Pages.Admin
{
    public class SettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SettingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Ensure default settings exist (idempotent)
            await SeedDefaultSettingsAsync();
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
                new AdminSetting { Key = "contact_map_location", Value = "1 Royd Ln, Esplanade, Taltala, Kolkata, West Bengal 700016", Description = "Google Maps Location (address or place name)" },

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
    }
}
