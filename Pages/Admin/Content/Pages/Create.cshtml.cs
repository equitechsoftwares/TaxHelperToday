using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Pages
{
    public class CreateModel : PageModel
    {
        private readonly IPageService _pageService;

        public CreateModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        [BindProperty]
        public CreatePageDto CreatePage { get; set; } = new();

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
                await _pageService.CreateAsync(CreatePage);
                TempData["SuccessMessage"] = "Page created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating page: {ex.Message}");
                return Page();
            }
        }
    }
}
