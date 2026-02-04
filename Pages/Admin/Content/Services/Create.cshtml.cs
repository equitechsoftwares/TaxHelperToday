using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Services
{
    public class CreateModel : PageModel
    {
        private readonly IServiceService _serviceService;

        private readonly IWebHostEnvironment _env;

        public CreateModel(IServiceService serviceService, IWebHostEnvironment env)
        {
            _serviceService = serviceService;
            _env = env;
        }

        [BindProperty]
        public CreateServiceDto Service { get; set; } = new();

        [BindProperty]
        public IFormFile? IconFile { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Handle icon upload (optional)
            if (IconFile != null && IconFile.Length > 0)
            {
                var savedPath = await SaveIconAsync(IconFile);
                if (savedPath == null)
                {
                    return Page();
                }

                Service.IconUrl = savedPath;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _serviceService.CreateAsync(Service);
                TempData["SuccessMessage"] = "Service created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating service: {ex.Message}");
                return Page();
            }
        }

        private async Task<string?> SaveIconAsync(IFormFile file)
        {
            const long maxBytes = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxBytes)
            {
                ModelState.AddModelError(nameof(IconFile), "Image must be 5MB or smaller.");
                return null;
            }

            var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp",
                "image/gif"
            };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(nameof(IconFile), "Only JPG, PNG, WebP, or GIF images are allowed.");
                return null;
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "services");
            Directory.CreateDirectory(uploadsRoot);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
            {
                ext = file.ContentType.ToLowerInvariant() switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    "image/gif" => ".gif",
                    _ => ".img"
                };
            }

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var physicalPath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = System.IO.File.Create(physicalPath))
            {
                await file.CopyToAsync(stream);
            }

            // Public URL path
            return $"/uploads/services/{fileName}";
        }
    }
}
