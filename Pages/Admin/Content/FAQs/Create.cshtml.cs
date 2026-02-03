using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.FAQs
{
    public class CreateModel : PageModel
    {
        private readonly IFaqService _faqService;

        public CreateModel(IFaqService faqService)
        {
            _faqService = faqService;
        }

        [BindProperty]
        public CreateFaqDto Faq { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _faqService.CreateAsync(Faq);
                TempData["SuccessMessage"] = "FAQ created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating FAQ: {ex.Message}");
                return Page();
            }
        }
    }
}
