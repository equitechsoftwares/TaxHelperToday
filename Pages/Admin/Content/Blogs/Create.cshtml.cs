using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly IBlogService _blogService;

        public CreateModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [BindProperty]
        public CreateBlogPostDto BlogPost { get; set; } = new();

        public void OnGet()
        {
        }

        [BindProperty]
        public string TagsInput { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
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
    }
}
