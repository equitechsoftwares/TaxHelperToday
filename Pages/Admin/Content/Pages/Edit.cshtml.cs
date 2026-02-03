using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Pages
{
    public class EditModel : PageModel
    {
        private readonly IPageService _pageService;

        public EditModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        [BindProperty]
        public UpdatePageDto UpdatePage { get; set; } = new();

        public PageDto? CurrentPage { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            CurrentPage = await _pageService.GetByIdAsync(id);

            if (CurrentPage == null)
            {
                return NotFound();
            }

            // Populate the form with existing data
            UpdatePage.Slug = CurrentPage.Slug;
            UpdatePage.Title = CurrentPage.Title;
            UpdatePage.Eyebrow = CurrentPage.Eyebrow;
            UpdatePage.HeroTitle = CurrentPage.HeroTitle;
            UpdatePage.HeroText = CurrentPage.HeroText;
            UpdatePage.Content = CurrentPage.Content;
            UpdatePage.MetaDescription = CurrentPage.MetaDescription;
            UpdatePage.MetaKeywords = CurrentPage.MetaKeywords;
            UpdatePage.LastUpdated = CurrentPage.LastUpdated;
            UpdatePage.IsActive = CurrentPage.IsActive;
            
            // Populate About page specific fields
            UpdatePage.StatsJson = CurrentPage.StatsJson;
            UpdatePage.OurStoryContent = CurrentPage.OurStoryContent;
            UpdatePage.HowWeWorkContent = CurrentPage.HowWeWorkContent;
            UpdatePage.HowWeWorkChecklistJson = CurrentPage.HowWeWorkChecklistJson;
            UpdatePage.MissionEyebrow = CurrentPage.MissionEyebrow;
            UpdatePage.MissionTitle = CurrentPage.MissionTitle;
            UpdatePage.MissionCardsJson = CurrentPage.MissionCardsJson;
            UpdatePage.WhoWeServeContent = CurrentPage.WhoWeServeContent;
            UpdatePage.WhoWeServeCategoriesJson = CurrentPage.WhoWeServeCategoriesJson;
            UpdatePage.ValuesJson = CurrentPage.ValuesJson;
            UpdatePage.TeamMembersJson = CurrentPage.TeamMembersJson;
            
            // Populate Home page specific fields
            UpdatePage.HeroTrustBadgesJson = CurrentPage.HeroTrustBadgesJson;
            UpdatePage.HeroMetricsJson = CurrentPage.HeroMetricsJson;
            UpdatePage.StatsBannerJson = CurrentPage.StatsBannerJson;
            UpdatePage.CtaBannerJson = CurrentPage.CtaBannerJson;
            UpdatePage.HowItWorksJson = CurrentPage.HowItWorksJson;
            UpdatePage.WhySectionJson = CurrentPage.WhySectionJson;
            UpdatePage.PricingPlansJson = CurrentPage.PricingPlansJson;
            UpdatePage.TrustSectionJson = CurrentPage.TrustSectionJson;
            UpdatePage.TestimonialsJson = CurrentPage.TestimonialsJson;
            UpdatePage.ValuePropositionJson = CurrentPage.ValuePropositionJson;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id, bool isActive)
        {
            // Set IsActive from the checkbox value
            UpdatePage.IsActive = isActive;

            if (!ModelState.IsValid)
            {
                CurrentPage = await _pageService.GetByIdAsync(id);
                if (CurrentPage == null)
                {
                    return NotFound();
                }
                return Page();
            }

            try
            {
                await _pageService.UpdateAsync(id, UpdatePage);
                TempData["SuccessMessage"] = "Page updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating page: {ex.Message}");
                CurrentPage = await _pageService.GetByIdAsync(id);
                if (CurrentPage == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }
    }
}
