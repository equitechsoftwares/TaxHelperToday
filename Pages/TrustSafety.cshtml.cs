using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages
{
    public class TrustSafetyModel : PageModel
    {
        private readonly IPageService _pageService;

        public TrustSafetyModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        public string? Content { get; set; }
        public string? Eyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroText { get; set; }
        public string? LastUpdated { get; set; }
        public string Title { get; set; } = "Trust & Safety";

        public async Task OnGetAsync()
        {
            var page = await _pageService.GetBySlugAsync("trust-safety");
            if (page != null)
            {
                Title = page.Title;
                Eyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroText = page.HeroText;
                Content = page.Content;
                LastUpdated = page.LastUpdated;
            }
        }
    }
}
