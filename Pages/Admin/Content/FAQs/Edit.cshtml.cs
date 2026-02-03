using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.FAQs
{
    public class EditModel : PageModel
    {
        private readonly IFaqService _faqService;

        public EditModel(IFaqService faqService)
        {
            _faqService = faqService;
        }

        [BindProperty]
        public UpdateFaqDto Faq { get; set; } = new();

        public FaqDto? CurrentFaq { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            CurrentFaq = await _faqService.GetByIdAsync(id);

            if (CurrentFaq == null)
            {
                return NotFound();
            }

            // Populate the form with existing data
            Faq.Question = CurrentFaq.Question;
            Faq.Answer = CurrentFaq.Answer;
            Faq.Category = CurrentFaq.Category;
            Faq.DisplayOrder = CurrentFaq.DisplayOrder;
            Faq.IsActive = CurrentFaq.IsActive;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id, bool isActive)
        {
            // Set IsActive from the checkbox value
            Faq.IsActive = isActive;

            if (!ModelState.IsValid)
            {
                CurrentFaq = await _faqService.GetByIdAsync(id);
                if (CurrentFaq == null)
                {
                    return NotFound();
                }
                return Page();
            }

            try
            {
                await _faqService.UpdateAsync(id, Faq);
                TempData["SuccessMessage"] = "FAQ updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating FAQ: {ex.Message}");
                CurrentFaq = await _faqService.GetByIdAsync(id);
                if (CurrentFaq == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }
    }
}
