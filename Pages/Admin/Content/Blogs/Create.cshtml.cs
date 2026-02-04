using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly IBlogService _blogService;
        private readonly IWebHostEnvironment _env;

        public CreateModel(IBlogService blogService, IWebHostEnvironment env)
        {
            _blogService = blogService;
            _env = env;
        }

        [BindProperty]
        public CreateBlogPostDto BlogPost { get; set; } = new();

        [BindProperty]
        public IFormFile? FeaturedImageFile { get; set; }

        public void OnGet()
        {
        }

        [BindProperty]
        public string TagsInput { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            // Handle featured image upload (optional)
            if (FeaturedImageFile != null && FeaturedImageFile.Length > 0)
            {
                var savedPath = await SaveFeaturedImageAsync(FeaturedImageFile);
                if (savedPath == null)
                {
                    return Page();
                }

                BlogPost.FeaturedImageUrl = savedPath;
            }

            // Validate tags are provided
            if (string.IsNullOrWhiteSpace(TagsInput))
            {
                ModelState.AddModelError(nameof(TagsInput), "Tags are required.");
                return Page();
            }

            // Parse tags from comma-separated string
            BlogPost.Tags = TagsInput.Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            // Validate at least one tag was parsed
            if (!BlogPost.Tags.Any())
            {
                ModelState.AddModelError(nameof(TagsInput), "At least one tag is required.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    return Page();
                }

                await _blogService.CreateAsync(BlogPost, userId);
                TempData["SuccessMessage"] = "Blog post created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating blog post: {ex.Message}");
                return Page();
            }
        }

        private async Task<string?> SaveFeaturedImageAsync(IFormFile file)
        {
            const long maxBytes = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxBytes)
            {
                ModelState.AddModelError(nameof(FeaturedImageFile), "Image must be 5MB or smaller.");
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
                ModelState.AddModelError(nameof(FeaturedImageFile), "Only JPG, PNG, WebP, or GIF images are allowed.");
                return null;
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "blogs");
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
            return $"/uploads/blogs/{fileName}";
        }
    }
}
