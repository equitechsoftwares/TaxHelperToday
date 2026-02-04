using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Services
{
    public class EditModel : PageModel
    {
        private readonly IServiceService _serviceService;

        private readonly IWebHostEnvironment _env;

        public EditModel(IServiceService serviceService, IWebHostEnvironment env)
        {
            _serviceService = serviceService;
            _env = env;
        }

        [BindProperty]
        public UpdateServiceDto Service { get; set; } = new();

        [BindProperty]
        public IFormFile? IconFile { get; set; }

        public ServiceDto? CurrentService { get; set; }
        
        public bool IsActiveValue => CurrentService?.IsActive ?? false;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            CurrentService = await _serviceService.GetByIdAsync(id);

            if (CurrentService == null)
            {
                return NotFound();
            }

            // Populate the form with existing data
            Service.Slug = CurrentService.Slug;
            Service.Name = CurrentService.Name;
            Service.Description = CurrentService.Description;
            Service.Content = CurrentService.Content;
            Service.Type = CurrentService.Type;
            Service.Level = CurrentService.Level;
            Service.Highlight = CurrentService.Highlight;
            Service.IconUrl = CurrentService.IconUrl;
            Service.IsActive = CurrentService.IsActive;
            Service.DisplayOrder = CurrentService.DisplayOrder;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id, bool isActive)
        {
            // Set IsActive from the checkbox value
            Service.IsActive = isActive;

            // Handle icon upload (optional)
            if (IconFile != null && IconFile.Length > 0)
            {
                var savedPath = await SaveIconAsync(IconFile);
                if (savedPath == null)
                {
                    CurrentService = await _serviceService.GetByIdAsync(id);
                    if (CurrentService == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }

                Service.IconUrl = savedPath;
            }

            if (!ModelState.IsValid)
            {
                CurrentService = await _serviceService.GetByIdAsync(id);
                if (CurrentService == null)
                {
                    return NotFound();
                }
                return Page();
            }

            try
            {
                await _serviceService.UpdateAsync(id, Service);
                TempData["SuccessMessage"] = "Service updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating service: {ex.Message}");
                CurrentService = await _serviceService.GetByIdAsync(id);
                if (CurrentService == null)
                {
                    return NotFound();
                }
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
