using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages
{
    public class FaqsModel : PageModel
    {
        private readonly IFaqService _faqService;
        private readonly IPageService _pageService;

        public FaqsModel(IFaqService faqService, IPageService pageService)
        {
            _faqService = faqService;
            _pageService = pageService;
        }

        public List<FaqDto> AllFaqs { get; set; } = new();
        public Dictionary<string, List<FaqDto>> FaqsByCategory { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string? HeroEyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroText { get; set; }
        public string? HelpCardTitle { get; set; }
        public string? HelpCardDescription { get; set; }
        public string? HelpCardButtonText { get; set; }

        public async Task OnGetAsync()
        {
            AllFaqs = (await _faqService.GetAllAsync(activeOnly: true)).ToList();
            
            Categories = AllFaqs
                .Where(f => !string.IsNullOrEmpty(f.Category))
                .Select(f => f.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            foreach (var category in Categories)
            {
                FaqsByCategory[category] = AllFaqs
                    .Where(f => f.Category == category)
                    .ToList();
            }

            // Load hero and help card content from page entity
            var page = await _pageService.GetBySlugAsync("faqs");
            if (page != null)
            {
                HeroEyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroText = page.HeroText;
                HelpCardTitle = page.FaqHelpCardTitle;
                HelpCardDescription = page.FaqHelpCardDescription;
                HelpCardButtonText = page.FaqHelpCardButtonText;
            }
        }
    }
}
