using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Contact.Domain.Entities;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceService _serviceService;
        private readonly IPageService _pageService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(ApplicationDbContext context, IServiceService serviceService, IPageService pageService, IEmailService emailService, ILogger<ContactModel> logger)
        {
            _context = context;
            _serviceService = serviceService;
            _pageService = pageService;
            _emailService = emailService;
            _logger = logger;
        }

        public string? Eyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroText { get; set; }
        public string? ContactFormTitle { get; set; }
        public string? ContactFormDescription { get; set; }
        public string? ContactFormButtonText { get; set; }
        public string? ContactFormNote { get; set; }
        public string? ContactReachUsTitle { get; set; }
        public string? ContactFindUsTitle { get; set; }
        public string? ContactFindUsDescription { get; set; }
        public string Title { get; set; } = "Contact Us";

        // Contact information from settings
        public string ContactEmail { get; set; } = "mrhoque64@gmail.com";
        public string ContactPhone { get; set; } = "+91-89103-97497";
        public string WorkingHours { get; set; } = "Monday to Saturday, 10:00 AM – 7:00 PM IST";
        public string WhatsAppNumber { get; set; } = "918910397497";
        public string WhatsAppMessage { get; set; } = "Hi, I'd like to get in touch with TaxHelperToday";
        public string OfficeAddress { get; set; } = "TaxHelperToday<br>1, Royd Ln, Esplanade<br>Taltala, Kolkata, West Bengal - 700016<br>India";
        public string MapUrl { get; set; } = "https://www.google.com/maps?q=1+Royd+Ln,+Esplanade,+Taltala,+Kolkata,+West+Bengal+700016&output=embed";
        public string MapDirectionsUrl { get; set; } = "https://www.google.com/maps/place/1,+Royd+Ln,+Esplanade,+Taltala,+Kolkata,+West+Bengal+700016/@@22.5517171,88.3537581,17z/data=!3m1!4b1!4m5!3m4!1s0x3a02771b30ffd405:0x30a20c3cf4c869fd!8m2!3d22.5517122!4d88.356333?entry=ttu";

        public async Task OnGetAsync()
        {
            // Load hero section data from database
            var page = await _pageService.GetBySlugAsync("contact");
            if (page != null)
            {
                Title = page.Title;
                Eyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroText = page.HeroText;
                ContactFormTitle = page.ContactFormTitle;
                ContactFormDescription = page.ContactFormDescription;
                ContactFormButtonText = page.ContactFormButtonText;
                ContactFormNote = page.ContactFormNote;
                ContactReachUsTitle = page.ContactReachUsTitle;
                ContactFindUsTitle = page.ContactFindUsTitle;
                ContactFindUsDescription = page.ContactFindUsDescription;
            }

            // Load contact information from settings
            var contactSettings = await _context.AdminSettings
                .Where(s => s.Key.StartsWith("contact_"))
                .ToListAsync();

            foreach (var setting in contactSettings)
            {
                switch (setting.Key)
                {
                    case "contact_email":
                        ContactEmail = setting.Value ?? ContactEmail;
                        break;
                    case "contact_phone":
                        ContactPhone = setting.Value ?? ContactPhone;
                        break;
                    case "contact_working_hours":
                        WorkingHours = setting.Value ?? WorkingHours;
                        break;
                    case "contact_whatsapp_number":
                        WhatsAppNumber = setting.Value ?? WhatsAppNumber;
                        break;
                    case "contact_whatsapp_message":
                        WhatsAppMessage = setting.Value ?? WhatsAppMessage;
                        break;
                    case "contact_office_address":
                        OfficeAddress = (setting.Value ?? OfficeAddress).Replace("\n", "<br>");
                        break;
                    case "contact_map_url":
                        MapUrl = setting.Value ?? MapUrl;
                        break;
                    case "contact_map_directions_url":
                        MapDirectionsUrl = setting.Value ?? MapDirectionsUrl;
                        break;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string name, string email, string phone, string subject, string message, string service, string type)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage("/Contact");
            }

            try
            {
                // Handle full contact form - message is required
                if (string.IsNullOrWhiteSpace(message))
                {
                    TempData["ErrorMessage"] = "Please fill in all required fields.";
                    return RedirectToPage("/Contact");
                }

                long? serviceId = null;
                // service parameter can be either a slug or service name
                if (!string.IsNullOrEmpty(service))
                {
                    var serviceEntity = await _serviceService.GetBySlugAsync(service);
                    if (serviceEntity == null)
                    {
                        // Try to find by name
                        var allServices = await _serviceService.GetAllAsync(activeOnly: true);
                        serviceEntity = allServices.FirstOrDefault(s => s.Name.Equals(service, StringComparison.OrdinalIgnoreCase));
                    }
                    serviceId = serviceEntity?.Id;
                }

                var enquiry = new ContactEnquiry
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Subject = subject,
                    Message = message,
                    EnquiryType = !string.IsNullOrEmpty(service) ? "Service Inquiry" : "General Inquiry",
                    ServiceId = serviceId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ContactEnquiries.Add(enquiry);
                await _context.SaveChangesAsync();

                // Send email notifications
                try
                {
                    var serviceName = serviceId.HasValue ? (await _serviceService.GetByIdAsync(serviceId.Value))?.Name : null;
                    
                    // Send notification to admin
                    await _emailService.SendEnquiryNotificationAsync(
                        enquiry.EnquiryType,
                        enquiry.Name,
                        enquiry.Email,
                        enquiry.Phone,
                        enquiry.Subject,
                        enquiry.Message,
                        serviceName
                    );

                    // Send confirmation to user
                    await _emailService.SendEnquiryConfirmationAsync(
                        enquiry.Email,
                        enquiry.Name,
                        enquiry.EnquiryType
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email notifications for enquiry {EnquiryId}", enquiry.Id);
                    // Don't fail the enquiry submission if email fails
                }

                TempData["SuccessMessage"] = "Thank you for contacting us! We'll get back to you within one business day.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting your enquiry. Please try again or contact us directly.";
            }

            return RedirectToPage("/Contact");
        }
    }
}
