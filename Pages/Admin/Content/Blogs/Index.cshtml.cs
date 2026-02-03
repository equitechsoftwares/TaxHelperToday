using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly IBlogService _blogService;

        public IndexModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public List<BlogPostDto> BlogPosts { get; set; } = new();

        public async Task OnGetAsync()
        {
            var blogs = await _blogService.GetAllAsync();
            BlogPosts = blogs.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            try
            {
                await _blogService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Blog post deleted successfully!";
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error deleting blog post.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostPublishAsync(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    TempData["ErrorMessage"] = "User not authenticated.";
                    return RedirectToPage();
                }

                await _blogService.PublishAsync(id, userId);
                TempData["SuccessMessage"] = "Blog post published successfully!";
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error publishing blog post.";
                return RedirectToPage();
            }
        }
    }
}
