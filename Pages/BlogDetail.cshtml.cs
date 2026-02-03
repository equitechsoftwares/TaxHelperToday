using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages
{
    public class BlogDetailModel : PageModel
    {
        private readonly IBlogService _blogService;

        public BlogDetailModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public BlogPostDto? BlogPost { get; set; }
        public List<BlogPostDto> RelatedBlogs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToPage("/Blogs");
            }

            BlogPost = await _blogService.GetBySlugAsync(slug);

            if (BlogPost == null)
            {
                return NotFound();
            }

            // Load related blogs (same category, excluding current)
            var allBlogs = await _blogService.GetAllAsync(publishedOnly: true);
            RelatedBlogs = allBlogs
                .Where(b => b.Id != BlogPost.Id && 
                           (!string.IsNullOrEmpty(BlogPost.Category) && b.Category == BlogPost.Category))
                .Take(3)
                .ToList();

            // Increment view count
            await _blogService.IncrementViewCountAsync(BlogPost.Id);

            return Page();
        }
    }
}
