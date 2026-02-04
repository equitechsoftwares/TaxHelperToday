using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Pages.Admin
{
    public class ContactSettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactSettingsModel> _logger;

        public ContactSettingsModel(
            ApplicationDbContext context,
            ILogger<ContactSettingsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<AdminSetting> Settings { get; set; } = new();

        [BindProperty]
        public Dictionary<string, string> SettingValues { get; set; } = new();

        public async Task OnGetAsync()
        {
            await SeedDefaultSettingsAsync();

            Settings = await _context.AdminSettings
                // Do not show legacy map URL fields in the UI.
                .Where(s => s.Key.StartsWith("contact_") &&
                            s.Key != "contact_map_url" &&
                            s.Key != "contact_map_directions_url")
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
                new AdminSetting { Key = "contact_email", Value = "mrhoque64@gmail.com", Description = "Contact Email" },
                new AdminSetting { Key = "contact_phone", Value = "+91-89103-97497", Description = "Contact Phone" },
                new AdminSetting { Key = "contact_working_hours", Value = "Monday to Saturday, 10:00 AM – 7:00 PM IST", Description = "Working Hours" },
                new AdminSetting { Key = "contact_whatsapp_number", Value = "918910397497", Description = "WhatsApp Number (without + or spaces)" },
                new AdminSetting { Key = "contact_whatsapp_message", Value = "Hi, I'd like to get in touch with TaxHelperToday", Description = "WhatsApp Pre-filled Message" },
                new AdminSetting { Key = "contact_office_address", Value = "TaxHelperToday\n1, Royd Ln, Esplanade\nTaltala, Kolkata, West Bengal - 700016\nIndia", Description = "Office Address" },
                // Single canonical map location; the site can generate both embed + directions URLs from this.
                new AdminSetting { Key = "contact_map_location", Value = "1 Royd Ln, Esplanade, Taltala, Kolkata, West Bengal 700016", Description = "Google Maps Location (address or place name)" }
            };

            var newSettings = defaultSettings
                .Where(s => !existingKeys.Contains(s.Key))
                .ToList();

            if (newSettings.Any())
            {
                _context.AdminSettings.AddRange(newSettings);
                await _context.SaveChangesAsync();
            }

            // Remove legacy map URL settings if they exist (we now generate both URLs from contact_map_location).
            var legacyMapSettings = existingSettings
                .Where(s => s.Key == "contact_map_url" || s.Key == "contact_map_directions_url")
                .ToList();

            if (legacyMapSettings.Any())
            {
                _context.AdminSettings.RemoveRange(legacyMapSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("contact_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }

            try
            {
                var settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("contact_"))
                    .ToListAsync();

                foreach (var kvp in SettingValues)
                {
                    if (!kvp.Key.StartsWith("contact_"))
                        continue;

                    var setting = settings.FirstOrDefault(s => s.Key == kvp.Key);
                    if (setting != null)
                    {
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

                TempData["SuccessMessage"] = "Contact settings updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact settings");
                ModelState.AddModelError(string.Empty, $"Error updating settings: {ex.Message}");
                Settings = await _context.AdminSettings
                    .Where(s => s.Key.StartsWith("contact_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }
        }
    }
}
