using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPageService _pageService;

        public IndexModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        public List<PageDto> Pages { get; set; } = new();

        public async Task OnGetAsync()
        {
            var pages = await _pageService.GetAllAsync();
            Pages = pages.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            try
            {
                var result = await _pageService.DeleteAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Page deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Page not found.";
                }
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error deleting page.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(long id)
        {
            try
            {
                var result = await _pageService.ToggleActiveAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Page status updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Page not found.";
                }
                return RedirectToPage();
            }
            catch
            {
                TempData["ErrorMessage"] = "Error updating page status.";
                return RedirectToPage();
            }
        }
    }
}
