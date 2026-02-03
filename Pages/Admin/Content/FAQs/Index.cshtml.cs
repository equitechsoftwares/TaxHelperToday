using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.FAQs
{
    public class IndexModel : PageModel
    {
        private readonly IFaqService _faqService;

        public IndexModel(IFaqService faqService)
        {
            _faqService = faqService;
        }

        public List<FaqDto> Faqs { get; set; } = new();

        public async Task OnGetAsync()
        {
            var faqs = await _faqService.GetAllAsync();
            Faqs = faqs.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            try
            {
                await _faqService.DeleteAsync(id);
                TempData["SuccessMessage"] = "FAQ deleted successfully!";
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error deleting FAQ.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(long id)
        {
            try
            {
                await _faqService.ToggleActiveAsync(id);
                TempData["SuccessMessage"] = "FAQ status updated successfully!";
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error updating FAQ status.";
                return RedirectToPage();
            }
        }
    }
}
