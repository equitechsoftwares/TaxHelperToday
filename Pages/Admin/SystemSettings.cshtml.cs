using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Pages.Admin
{
    public class SystemSettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemSettingsModel> _logger;
        private readonly IWebHostEnvironment _environment;

        private static readonly HashSet<string> AllowedLogoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".webp"
        };

        private const long MaxLogoUploadBytes = 5 * 1024 * 1024; // 5MB

        public SystemSettingsModel(
            ApplicationDbContext context,
            ILogger<SystemSettingsModel> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public List<AdminSetting> Settings { get; set; } = new();

        [BindProperty]
        public Dictionary<string, string> SettingValues { get; set; } = new();

        [BindProperty]
        public IFormFile? SiteLogoFile { get; set; }

        public async Task OnGetAsync()
        {
            await SeedDefaultSettingsAsync();

            Settings = await _context.AdminSettings
                .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
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
                new AdminSetting { Key = "site_name", Value = "TaxHelperToday", Description = "Site Name" },
                new AdminSetting { Key = "site_logo_url", Value = "~/img/logo.png", Description = "Site Logo URL/Path (used in header/footer + favicon)" },
                new AdminSetting { Key = "header_logo_subtitle", Value = "Compliance. Clarity. Confidence.", Description = "Header logo subtitle/tagline" },
                new AdminSetting { Key = "footer_logo_subtitle", Value = "CA-Led Tax Compliance", Description = "Footer logo subtitle/tagline" },
                new AdminSetting { Key = "footer_description", Value = "{{site_name}} is an independent tax and compliance advisory platform. We help individuals and businesses understand and meet their obligations under Indian tax laws.", Description = "Footer description (supports {{site_name}} placeholder; HTML allowed)" },
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
                    .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }

            try
            {
                // Handle logo upload (optional). If provided, it overrides site_logo_url.
                if (SiteLogoFile != null && SiteLogoFile.Length > 0)
                {
                    if (SiteLogoFile.Length > MaxLogoUploadBytes)
                    {
                        ModelState.AddModelError(string.Empty, "Logo file is too large. Max allowed size is 5MB.");
                        Settings = await _context.AdminSettings
                            .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
                            .OrderBy(s => s.Key)
                            .ToListAsync();
                        return Page();
                    }

                    var ext = Path.GetExtension(SiteLogoFile.FileName) ?? string.Empty;
                    if (!AllowedLogoExtensions.Contains(ext))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid logo file type. Allowed: PNG, JPG, JPEG, WEBP.");
                        Settings = await _context.AdminSettings
                            .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
                            .OrderBy(s => s.Key)
                            .ToListAsync();
                        return Page();
                    }

                    var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "branding");
                    Directory.CreateDirectory(uploadDir);

                    var fileName = $"logo-{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext.ToLowerInvariant()}";
                    var fullPath = Path.Combine(uploadDir, fileName);

                    await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await SiteLogoFile.CopyToAsync(fs);
                    }

                    SettingValues["site_logo_url"] = $"~/uploads/branding/{fileName}";
                }

                var settings = await _context.AdminSettings
                    .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
                    .ToListAsync();

                foreach (var kvp in SettingValues)
                {
                    if (kvp.Key.StartsWith("contact_") || kvp.Key.StartsWith("smtp_"))
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

                TempData["SuccessMessage"] = "System settings updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system settings");
                ModelState.AddModelError(string.Empty, $"Error updating settings: {ex.Message}");
                Settings = await _context.AdminSettings
                    .Where(s => !s.Key.StartsWith("contact_") && !s.Key.StartsWith("smtp_"))
                    .OrderBy(s => s.Key)
                    .ToListAsync();
                return Page();
            }
        }
    }
}
