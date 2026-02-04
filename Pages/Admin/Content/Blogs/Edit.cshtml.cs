using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Blogs
{
    public class EditModel : PageModel
    {
        private readonly IBlogService _blogService;
        private readonly IWebHostEnvironment _env;

        public EditModel(IBlogService blogService, IWebHostEnvironment env)
        {
            _blogService = blogService;
            _env = env;
        }

        [BindProperty]
        public UpdateBlogPostDto BlogPost { get; set; } = new();

        [BindProperty]
        public IFormFile? FeaturedImageFile { get; set; }

        [BindProperty]
        public string TagsInput { get; set; } = string.Empty;

        public BlogPostDto? CurrentBlogPost { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            CurrentBlogPost = await _blogService.GetByIdAsync(id);

            if (CurrentBlogPost == null)
            {
                return NotFound();
            }

            // Populate the form with existing data
            BlogPost.Slug = CurrentBlogPost.Slug;
            BlogPost.Title = CurrentBlogPost.Title;
            BlogPost.Excerpt = CurrentBlogPost.Excerpt;
            BlogPost.Content = CurrentBlogPost.Content;
            BlogPost.Category = CurrentBlogPost.Category;
            BlogPost.ReadTime = CurrentBlogPost.ReadTime;
            BlogPost.FeaturedImageUrl = CurrentBlogPost.FeaturedImageUrl;
            BlogPost.MetaDescription = CurrentBlogPost.MetaDescription;
            BlogPost.MetaKeywords = CurrentBlogPost.MetaKeywords;
            BlogPost.IsPublished = CurrentBlogPost.IsPublished;
            BlogPost.Tags = CurrentBlogPost.Tags;

            // Set tags input for display
            TagsInput = string.Join(", ", CurrentBlogPost.Tags);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id, bool isPublished)
        {
            // Set IsPublished from the checkbox value
            BlogPost.IsPublished = isPublished;

            // Handle featured image upload (optional)
            if (FeaturedImageFile != null && FeaturedImageFile.Length > 0)
            {
                var savedPath = await SaveFeaturedImageAsync(FeaturedImageFile);
                if (savedPath == null)
                {
                    CurrentBlogPost = await _blogService.GetByIdAsync(id);
                    if (CurrentBlogPost == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }

                BlogPost.FeaturedImageUrl = savedPath;
            }

            // Validate tags are provided
            if (string.IsNullOrWhiteSpace(TagsInput))
            {
                ModelState.AddModelError(nameof(TagsInput), "Tags are required.");
                CurrentBlogPost = await _blogService.GetByIdAsync(id);
                if (CurrentBlogPost == null)
                {
                    return NotFound();
                }
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
                CurrentBlogPost = await _blogService.GetByIdAsync(id);
                if (CurrentBlogPost == null)
                {
                    return NotFound();
                }
                return Page();
            }

            if (!ModelState.IsValid)
            {
                CurrentBlogPost = await _blogService.GetByIdAsync(id);
                if (CurrentBlogPost == null)
                {
                    return NotFound();
                }
                return Page();
            }

            try
            {
                // Get user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    CurrentBlogPost = await _blogService.GetByIdAsync(id);
                    if (CurrentBlogPost == null)
                    {
                        return NotFound();
                    }
                    return Page();
                }

                await _blogService.UpdateAsync(id, BlogPost, userId);
                TempData["SuccessMessage"] = "Blog post updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating blog post: {ex.Message}");
                CurrentBlogPost = await _blogService.GetByIdAsync(id);
                if (CurrentBlogPost == null)
                {
                    return NotFound();
                }
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
