using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;

namespace TaxHelperToday.Pages.Admin.Content.Services
{
    public class EditModel : PageModel
    {
        private readonly IServiceService _serviceService;

        public EditModel(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [BindProperty]
        public UpdateServiceDto Service { get; set; } = new();

        public ServiceDto? CurrentService { get; set; }
        
        public bool IsActiveValue => CurrentService?.IsActive ?? false;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            CurrentService = await _serviceService.GetByIdAsync(id);

            if (CurrentService == null)
            {
                return NotFound();
            }

            // Populate the form with existing data
            Service.Slug = CurrentService.Slug;
            Service.Name = CurrentService.Name;
            Service.Description = CurrentService.Description;
            Service.Content = CurrentService.Content;
            Service.Type = CurrentService.Type;
            Service.Level = CurrentService.Level;
            Service.Highlight = CurrentService.Highlight;
            Service.IconUrl = CurrentService.IconUrl;
            Service.IsActive = CurrentService.IsActive;
            Service.DisplayOrder = CurrentService.DisplayOrder;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id, bool isActive)
        {
            // Set IsActive from the checkbox value
            Service.IsActive = isActive;
            
            if (!ModelState.IsValid)
            {
                CurrentService = await _serviceService.GetByIdAsync(id);
                if (CurrentService == null)
                {
                    return NotFound();
                }
                return Page();
            }

            try
            {
                await _serviceService.UpdateAsync(id, Service);
                return RedirectToPage("./Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating service: {ex.Message}");
                CurrentService = await _serviceService.GetByIdAsync(id);
                if (CurrentService == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }
    }
}
