using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Services
{
    public class IndexModel : PageModel
    {
        private readonly IServiceService _serviceService;

        public IndexModel(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public List<ServiceDto> Services { get; set; } = new();

        public async Task OnGetAsync()
        {
            var services = await _serviceService.GetAllAsync();
            Services = services.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            try
            {
                await _serviceService.DeleteAsync(id);
                return RedirectToPage();
            }
            catch
            {
                return Page();
            }
        }
    }
}
