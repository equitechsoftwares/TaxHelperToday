using System.ComponentModel.DataAnnotations;

namespace TaxHelperToday.Modules.Content.Domain.Entities;

public class Page
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Eyebrow { get; set; }
    
    [MaxLength(500)]
    public string? HeroTitle { get; set; }
    
    public string? HeroText { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? MetaDescription { get; set; }
    
    [MaxLength(500)]
    public string? MetaKeywords { get; set; }
    
    [MaxLength(100)]
    public string? LastUpdated { get; set; }
    
    // About page specific fields
    public string? StatsJson { get; set; } // JSON array of stats: { icon, value, label, description }
    [MaxLength(200)]
    public string? OurStoryTitle { get; set; } // Section heading for "Our Story"
    public string? OurStoryContent { get; set; } // Text content for "Our Story" section
    [MaxLength(200)]
    public string? HowWeWorkTitle { get; set; } // Section heading for "How We Work"
    public string? HowWeWorkContent { get; set; } // Text content for "How We Work" section
    public string? HowWeWorkChecklistJson { get; set; } // JSON array of checklist items: { text }
    [MaxLength(200)]
    public string? MissionEyebrow { get; set; } // Eyebrow text for "Our Mission" section
    [MaxLength(500)]
    public string? MissionTitle { get; set; } // Title for "Our Mission" section
    public string? MissionCardsJson { get; set; } // JSON array of mission cards: { icon, title, description }
    [MaxLength(200)]
    public string? WhoWeServeTitle { get; set; } // Section heading for "Who We Serve"
    public string? WhoWeServeContent { get; set; } // Text content for "Who we serve" section
    public string? WhoWeServeCategoriesJson { get; set; } // JSON array of category badges: { icon, label }
    [MaxLength(200)]
    public string? WhatWeStandForTitle { get; set; } // Section heading for "What We Stand For"
    public string? ValuesJson { get; set; } // JSON array of values: { icon, title, subtitle }
    [MaxLength(200)]
    public string? TeamSectionTitle { get; set; } // Section heading for "Team"
    [MaxLength(500)]
    public string? TeamSectionSubtitle { get; set; } // Subtitle for "Team" section
    public string? TeamMembersJson { get; set; } // JSON array of team members: { name, role, bio, avatar }
    
    // Home page specific fields
    public string? HeroTrustBadgesJson { get; set; } // JSON array: { icon, value, label }
    public string? HeroMetricsJson { get; set; } // JSON array: { metric, value, label }
    [MaxLength(100)]
    public string? MiniEnquiryBadge { get; set; } // Badge text for mini-enquiry card (e.g., "Free Consultation")
    [MaxLength(200)]
    public string? MiniEnquiryTitle { get; set; } // Title for mini-enquiry card
    public string? MiniEnquiryDescription { get; set; } // Description text for mini-enquiry card
    [MaxLength(100)]
    public string? MiniEnquiryButtonText { get; set; } // Button text for mini-enquiry card
    [MaxLength(500)]
    public string? MiniEnquiryNote { get; set; } // Note text below mini-enquiry form
    public string? StatsBannerJson { get; set; } // JSON: { eyebrow, title, subtitle, stats: [{ icon, value, label, description }] }
    public string? CtaBannerJson { get; set; } // JSON: { title, text, buttonText }
    public string? HowItWorksJson { get; set; } // JSON: { eyebrow, title, subtitle, steps: [{ number, icon, title, text }] }
    public string? WhySectionJson { get; set; } // JSON: { eyebrow, title, subtitle, features: [{ icon, title, text }] }
    public string? PricingPlansJson { get; set; } // JSON: { eyebrow, title, subtitle, plans: [{ name, price, period, description, features: [string], isFeatured, badge }] }
    public string? TrustSectionJson { get; set; } // JSON: { eyebrow, title, text, listItems: [string], badge: { label, title }, badgeText }
    public string? TestimonialsJson { get; set; } // JSON: { eyebrow, title, subtitle, testimonials: [{ rating, text, author: { name, role } }] }
    public string? ValuePropositionJson { get; set; } // JSON: { eyebrow, title, items: [{ icon, title, text }] }
    
    // Contact page specific fields
    [MaxLength(200)]
    public string? ContactFormTitle { get; set; } // Title for contact form section
    public string? ContactFormDescription { get; set; } // Description for contact form section
    [MaxLength(100)]
    public string? ContactFormButtonText { get; set; } // Button text for contact form
    [MaxLength(500)]
    public string? ContactFormNote { get; set; } // Note text below contact form
    [MaxLength(200)]
    public string? ContactReachUsTitle { get; set; } // Title for "Reach us" section
    [MaxLength(200)]
    public string? ContactFindUsTitle { get; set; } // Title for "Find us" section
    public string? ContactFindUsDescription { get; set; } // Description for "Find us" section
    
    // FAQs page specific fields
    [MaxLength(200)]
    public string? FaqHelpCardTitle { get; set; } // Title for "Still have questions?" help card
    public string? FaqHelpCardDescription { get; set; } // Description for help card
    [MaxLength(100)]
    public string? FaqHelpCardButtonText { get; set; } // Button text for help card
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
