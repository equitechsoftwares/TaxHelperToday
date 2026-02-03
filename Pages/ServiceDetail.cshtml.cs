using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Pages
{
    public class ServiceDetailModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ServiceDetailModel> _logger;

        public ServiceDetailModel(IServiceService serviceService, ApplicationDbContext context, IEmailService emailService, ILogger<ServiceDetailModel> logger)
        {
            _serviceService = serviceService;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public ServiceDto? Service { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToPage("/Index");
            }

            Service = await _serviceService.GetBySlugAsync(slug);

            if (Service == null)
            {
                return NotFound();
            }

            // Check for success/error messages from TempData
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string slug, string name, string email, string phone, string message)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToPage("/Index");
            }

            Service = await _serviceService.GetBySlugAsync(slug);

            if (Service == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage("/ServiceDetail", new { slug });
            }

            try
            {
                var enquiry = new ContactEnquiry
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Subject = $"Enquiry about {Service.Name}",
                    Message = message,
                    EnquiryType = "Service Inquiry",
                    ServiceId = Service.Id,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ContactEnquiries.Add(enquiry);
                await _context.SaveChangesAsync();

                // Send email notifications
                try
                {
                    // Send notification to admin
                    await _emailService.SendEnquiryNotificationAsync(
                        "Service Inquiry",
                        enquiry.Name,
                        enquiry.Email,
                        enquiry.Phone,
                        enquiry.Subject,
                        enquiry.Message,
                        Service.Name
                    );

                    // Send confirmation to user
                    await _emailService.SendEnquiryConfirmationAsync(
                        enquiry.Email,
                        enquiry.Name,
                        "Service Inquiry"
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email notifications for enquiry {EnquiryId}", enquiry.Id);
                    // Don't fail the enquiry submission if email fails
                }

                TempData["SuccessMessage"] = "Thank you for your enquiry! Our team will get back to you within one business day.";
                return RedirectToPage("/ServiceDetail", new { slug });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting your enquiry. Please try again later.";
                return RedirectToPage("/ServiceDetail", new { slug });
            }
        }
    }
}
