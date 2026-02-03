using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages
{
    public class BlogsModel : PageModel
    {
        private readonly IBlogService _blogService;
        private readonly IPageService _pageService;

        public BlogsModel(IBlogService blogService, IPageService pageService)
        {
            _blogService = blogService;
            _pageService = pageService;
        }

        public List<BlogPostDto> BlogPosts { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string? HeroEyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroText { get; set; }

        public async Task OnGetAsync(string category = null)
        {
            var allBlogs = await _blogService.GetAllAsync(publishedOnly: true);
            
            if (!string.IsNullOrEmpty(category))
            {
                BlogPosts = allBlogs.Where(b => b.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }
            else
            {
                BlogPosts = allBlogs.ToList();
            }

            Categories = allBlogs
                .Where(b => !string.IsNullOrEmpty(b.Category))
                .Select(b => b.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Load hero content from page entity
            var page = await _pageService.GetBySlugAsync("blogs");
            if (page != null)
            {
                HeroEyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroText = page.HeroText;
            }
        }
    }
}
