using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Pages.Admin.Contact
{
    public class MiniEnquiriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MiniEnquiriesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MiniEnquiryViewModel> MiniEnquiries { get; set; } = new();

        public async Task OnGetAsync()
        {
            var miniEnquiries = await _context.MiniEnquiries
                .Include(e => e.AssignedUser)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            MiniEnquiries = miniEnquiries.Select(e => new MiniEnquiryViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                UserType = e.UserType,
                Status = e.Status,
                AssignedToName = e.AssignedUser?.FullName,
                Notes = e.Notes,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(long id, string status)
        {
            var miniEnquiry = await _context.MiniEnquiries.FindAsync(id);
            if (miniEnquiry == null)
            {
                return NotFound();
            }

            miniEnquiry.Status = status;
            miniEnquiry.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mini enquiry status updated successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var miniEnquiry = await _context.MiniEnquiries.FindAsync(id);
            if (miniEnquiry == null)
            {
                return NotFound();
            }

            _context.MiniEnquiries.Remove(miniEnquiry);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mini enquiry deleted successfully!";
            return RedirectToPage();
        }

        public class MiniEnquiryViewModel
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? UserType { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? AssignedToName { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
