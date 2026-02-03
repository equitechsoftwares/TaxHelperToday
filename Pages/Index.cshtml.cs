using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Contact.Domain.Entities;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using System.Text.Json;

namespace TaxHelperToday.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IBlogService _blogService;
        private readonly IPageService _pageService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IServiceService serviceService, IBlogService blogService, IPageService pageService, ApplicationDbContext context, IEmailService emailService, ILogger<IndexModel> logger)
        {
            _serviceService = serviceService;
            _blogService = blogService;
            _pageService = pageService;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public List<ServiceDto> Services { get; set; } = new();
        public List<BlogPostDto> RecentBlogs { get; set; } = new();

        // Home page content from database
        public string? HeroEyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroSubtitle { get; set; }
        public List<HeroTrustBadge>? HeroTrustBadges { get; set; }
        public List<HeroMetric>? HeroMetrics { get; set; }
        public string? MiniEnquiryBadge { get; set; }
        public string? MiniEnquiryTitle { get; set; }
        public string? MiniEnquiryDescription { get; set; }
        public string? MiniEnquiryButtonText { get; set; }
        public string? MiniEnquiryNote { get; set; }
        public StatsBanner? StatsBanner { get; set; }
        public CtaBanner? CtaBanner { get; set; }
        public HowItWorks? HowItWorks { get; set; }
        public WhySection? WhySection { get; set; }
        public PricingSection? PricingSection { get; set; }
        public TrustSection? TrustSection { get; set; }
        public TestimonialsSection? TestimonialsSection { get; set; }
        public ValueProposition? ValueProposition { get; set; }

        public async Task OnGetAsync()
        {
            Services = (await _serviceService.GetAllAsync(activeOnly: true)).ToList();
            RecentBlogs = (await _blogService.GetAllAsync(publishedOnly: true)).Take(3).ToList();

            // Load home page from database
            var page = await _pageService.GetBySlugAsync("home") ?? await _pageService.GetBySlugAsync("index");
            if (page != null)
            {
                HeroEyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroSubtitle = page.HeroText;
                MiniEnquiryBadge = page.MiniEnquiryBadge;
                MiniEnquiryTitle = page.MiniEnquiryTitle;
                MiniEnquiryDescription = page.MiniEnquiryDescription;
                MiniEnquiryButtonText = page.MiniEnquiryButtonText;
                MiniEnquiryNote = page.MiniEnquiryNote;

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (!string.IsNullOrEmpty(page.HeroTrustBadgesJson))
                {
                    HeroTrustBadges = JsonSerializer.Deserialize<List<HeroTrustBadge>>(page.HeroTrustBadgesJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.HeroMetricsJson))
                {
                    HeroMetrics = JsonSerializer.Deserialize<List<HeroMetric>>(page.HeroMetricsJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.StatsBannerJson))
                {
                    StatsBanner = JsonSerializer.Deserialize<StatsBanner>(page.StatsBannerJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.CtaBannerJson))
                {
                    CtaBanner = JsonSerializer.Deserialize<CtaBanner>(page.CtaBannerJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.HowItWorksJson))
                {
                    HowItWorks = JsonSerializer.Deserialize<HowItWorks>(page.HowItWorksJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.WhySectionJson))
                {
                    WhySection = JsonSerializer.Deserialize<WhySection>(page.WhySectionJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.PricingPlansJson))
                {
                    PricingSection = JsonSerializer.Deserialize<PricingSection>(page.PricingPlansJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.TrustSectionJson))
                {
                    TrustSection = JsonSerializer.Deserialize<TrustSection>(page.TrustSectionJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.TestimonialsJson))
                {
                    TestimonialsSection = JsonSerializer.Deserialize<TestimonialsSection>(page.TestimonialsJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.ValuePropositionJson))
                {
                    ValueProposition = JsonSerializer.Deserialize<ValueProposition>(page.ValuePropositionJson, jsonOptions);
                }
            }
        }

        // Handler for mini enquiries from homepage
        public async Task<IActionResult> OnPostMiniEnquiryAsync(string name, string email, string type)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(type))
            {
                TempData["MiniEnquiryErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage("/Index");
            }

            try
            {
                var miniEnquiry = new MiniEnquiry
                {
                    Name = name.Trim(),
                    Email = email.Trim(),
                    UserType = type.Trim(),
                    Status = "New",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.MiniEnquiries.Add(miniEnquiry);
                await _context.SaveChangesAsync();

                // Send email notifications
                try
                {
                    // Send notification to admin
                    await _emailService.SendEnquiryNotificationAsync(
                        "Mini Enquiry",
                        miniEnquiry.Name,
                        miniEnquiry.Email,
                        null,
                        $"New Mini Enquiry - {miniEnquiry.UserType}",
                        $"User Type: {miniEnquiry.UserType}\n\nThis is a mini enquiry from the homepage. Please contact the user to discuss their tax needs.",
                        null
                    );

                    // Send confirmation to user
                    await _emailService.SendEnquiryConfirmationAsync(
                        miniEnquiry.Email,
                        miniEnquiry.Name,
                        "Mini Enquiry"
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email notifications for mini enquiry {EnquiryId}", miniEnquiry.Id);
                    // Don't fail the enquiry submission if email fails
                }

                TempData["MiniEnquirySuccessMessage"] = "Thank you for your interest! Our team will contact you within 24 hours.";
            }
            catch (Exception ex)
            {
                TempData["MiniEnquiryErrorMessage"] = "An error occurred while submitting your enquiry. Please try again.";
            }

            return RedirectToPage("/Index");
        }
    }

    // Home page data models
    public class HeroTrustBadge
    {
        public string Icon { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class HeroMetric
    {
        public string Metric { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class StatsBanner
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<StatItem>? Stats { get; set; }
    }

    public class CtaBanner
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? ButtonText { get; set; }
    }

    public class HowItWorks
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<ProcessStep>? Steps { get; set; }
    }

    public class ProcessStep
    {
        public int Number { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class WhySection
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<FeatureCard>? Features { get; set; }
    }

    public class FeatureCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class PricingSection
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<PricingPlan>? Plans { get; set; }
    }

    public class PricingPlan
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string>? Features { get; set; }
        public bool IsFeatured { get; set; }
        public string? Badge { get; set; }
    }

    public class TrustSection
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public List<string>? ListItems { get; set; }
        public TrustBadge? Badge { get; set; }
        public string? BadgeText { get; set; }
    }

    public class TrustBadge
    {
        public string Label { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class TestimonialsSection
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<Testimonial>? Testimonials { get; set; }
    }

    public class Testimonial
    {
        public string Rating { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public TestimonialAuthor? Author { get; set; }
    }

    public class TestimonialAuthor
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ValueProposition
    {
        public string? Eyebrow { get; set; }
        public string? Title { get; set; }
        public List<ValueItem>? Items { get; set; }
    }

    public class ValueItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
