using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Pages.Admin.Contact.Enquiries
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public EnquiryViewModel? Enquiry { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var enquiry = await _context.ContactEnquiries
                .Include(e => e.Service)
                .Include(e => e.AssignedUser)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enquiry == null)
            {
                return NotFound();
            }

            Enquiry = new EnquiryViewModel
            {
                Id = enquiry.Id,
                Name = enquiry.Name,
                Email = enquiry.Email,
                Phone = enquiry.Phone,
                Subject = enquiry.Subject,
                Message = enquiry.Message,
                EnquiryType = enquiry.EnquiryType,
                ServiceName = enquiry.Service?.Name,
                Status = enquiry.Status,
                AssignedToName = enquiry.AssignedUser?.FullName,
                Notes = enquiry.Notes,
                CreatedAt = enquiry.CreatedAt,
                UpdatedAt = enquiry.UpdatedAt
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(long id, string status)
        {
            var enquiry = await _context.ContactEnquiries.FindAsync(id);
            if (enquiry == null)
            {
                return NotFound();
            }

            enquiry.Status = status;
            enquiry.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enquiry status updated successfully!";
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostUpdateNotesAsync(long id, string notes)
        {
            var enquiry = await _context.ContactEnquiries.FindAsync(id);
            if (enquiry == null)
            {
                return NotFound();
            }

            enquiry.Notes = notes;
            enquiry.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Notes updated successfully!";
            return RedirectToPage(new { id });
        }

        public class EnquiryViewModel
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Subject { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? EnquiryType { get; set; }
            public string? ServiceName { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? AssignedToName { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
