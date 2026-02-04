using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Pages.Admin.Contact
{
    public class EnquiriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EnquiriesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<EnquiryViewModel> Enquiries { get; set; } = new();
        public string? SelectedStatus { get; set; }

        public async Task OnGetAsync(string status = null)
        {
            SelectedStatus = status;

            var query = _context.ContactEnquiries
                .Include(e => e.Service)
                .Include(e => e.AssignedUser)
                .AsQueryable();

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(e => e.Status != null && e.Status.ToLower() == status.ToLower());
            }

            var enquiries = await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            Enquiries = enquiries.Select(e => new EnquiryViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Phone = e.Phone,
                Subject = e.Subject,
                Message = e.Message,
                EnquiryType = e.EnquiryType,
                ServiceName = e.Service?.Name,
                Status = e.Status,
                AssignedToName = e.AssignedUser?.FullName,
                Notes = e.Notes,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
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
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var enquiry = await _context.ContactEnquiries.FindAsync(id);
            if (enquiry == null)
            {
                return NotFound();
            }

            _context.ContactEnquiries.Remove(enquiry);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enquiry deleted successfully!";
            return RedirectToPage();
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
