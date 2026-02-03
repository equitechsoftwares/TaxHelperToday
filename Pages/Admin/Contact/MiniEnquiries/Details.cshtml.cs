using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Pages.Admin.Contact.MiniEnquiries
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public MiniEnquiryViewModel? MiniEnquiry { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var miniEnquiry = await _context.MiniEnquiries
                .Include(e => e.AssignedUser)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miniEnquiry == null)
            {
                return NotFound();
            }

            MiniEnquiry = new MiniEnquiryViewModel
            {
                Id = miniEnquiry.Id,
                Name = miniEnquiry.Name,
                Email = miniEnquiry.Email,
                UserType = miniEnquiry.UserType,
                Status = miniEnquiry.Status,
                AssignedToName = miniEnquiry.AssignedUser?.FullName,
                Notes = miniEnquiry.Notes,
                CreatedAt = miniEnquiry.CreatedAt,
                UpdatedAt = miniEnquiry.UpdatedAt
            };

            return Page();
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
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostUpdateNotesAsync(long id, string notes)
        {
            var miniEnquiry = await _context.MiniEnquiries.FindAsync(id);
            if (miniEnquiry == null)
            {
                return NotFound();
            }

            miniEnquiry.Notes = notes;
            miniEnquiry.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Notes updated successfully!";
            return RedirectToPage(new { id });
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
