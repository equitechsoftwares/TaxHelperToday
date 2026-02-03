namespace TaxHelperToday.Modules.Content.Application.DTOs;

public class PageDto
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Eyebrow { get; set; }
    public string? HeroTitle { get; set; }
    public string? HeroText { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? LastUpdated { get; set; }
    // About page specific fields
    public string? StatsJson { get; set; }
    public string? OurStoryTitle { get; set; }
    public string? OurStoryContent { get; set; }
    public string? HowWeWorkTitle { get; set; }
    public string? HowWeWorkContent { get; set; }
    public string? HowWeWorkChecklistJson { get; set; }
    public string? MissionEyebrow { get; set; }
    public string? MissionTitle { get; set; }
    public string? MissionCardsJson { get; set; }
    public string? WhoWeServeTitle { get; set; }
    public string? WhoWeServeContent { get; set; }
    public string? WhoWeServeCategoriesJson { get; set; }
    public string? WhatWeStandForTitle { get; set; }
    public string? ValuesJson { get; set; }
    public string? TeamSectionTitle { get; set; }
    public string? TeamSectionSubtitle { get; set; }
    public string? TeamMembersJson { get; set; }
    // Home page specific fields
    public string? HeroTrustBadgesJson { get; set; }
    public string? HeroMetricsJson { get; set; }
    public string? MiniEnquiryBadge { get; set; }
    public string? MiniEnquiryTitle { get; set; }
    public string? MiniEnquiryDescription { get; set; }
    public string? MiniEnquiryButtonText { get; set; }
    public string? MiniEnquiryNote { get; set; }
    public string? StatsBannerJson { get; set; }
    public string? CtaBannerJson { get; set; }
    public string? HowItWorksJson { get; set; }
    public string? WhySectionJson { get; set; }
    public string? PricingPlansJson { get; set; }
    public string? TrustSectionJson { get; set; }
    public string? TestimonialsJson { get; set; }
    public string? ValuePropositionJson { get; set; }
    // Contact page specific fields
    public string? ContactFormTitle { get; set; }
    public string? ContactFormDescription { get; set; }
    public string? ContactFormButtonText { get; set; }
    public string? ContactFormNote { get; set; }
    public string? ContactReachUsTitle { get; set; }
    public string? ContactFindUsTitle { get; set; }
    public string? ContactFindUsDescription { get; set; }
    // FAQs page specific fields
    public string? FaqHelpCardTitle { get; set; }
    public string? FaqHelpCardDescription { get; set; }
    public string? FaqHelpCardButtonText { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePageDto
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Eyebrow { get; set; }
    public string? HeroTitle { get; set; }
    public string? HeroText { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? LastUpdated { get; set; }
    // About page specific fields
    public string? StatsJson { get; set; }
    public string? OurStoryTitle { get; set; }
    public string? OurStoryContent { get; set; }
    public string? HowWeWorkTitle { get; set; }
    public string? HowWeWorkContent { get; set; }
    public string? HowWeWorkChecklistJson { get; set; }
    public string? MissionEyebrow { get; set; }
    public string? MissionTitle { get; set; }
    public string? MissionCardsJson { get; set; }
    public string? WhoWeServeTitle { get; set; }
    public string? WhoWeServeContent { get; set; }
    public string? WhoWeServeCategoriesJson { get; set; }
    public string? WhatWeStandForTitle { get; set; }
    public string? ValuesJson { get; set; }
    public string? TeamSectionTitle { get; set; }
    public string? TeamSectionSubtitle { get; set; }
    public string? TeamMembersJson { get; set; }
    // Home page specific fields
    public string? HeroTrustBadgesJson { get; set; }
    public string? HeroMetricsJson { get; set; }
    public string? MiniEnquiryBadge { get; set; }
    public string? MiniEnquiryTitle { get; set; }
    public string? MiniEnquiryDescription { get; set; }
    public string? MiniEnquiryButtonText { get; set; }
    public string? MiniEnquiryNote { get; set; }
    public string? StatsBannerJson { get; set; }
    public string? CtaBannerJson { get; set; }
    public string? HowItWorksJson { get; set; }
    public string? WhySectionJson { get; set; }
    public string? PricingPlansJson { get; set; }
    public string? TrustSectionJson { get; set; }
    public string? TestimonialsJson { get; set; }
    public string? ValuePropositionJson { get; set; }
    // Contact page specific fields
    public string? ContactFormTitle { get; set; }
    public string? ContactFormDescription { get; set; }
    public string? ContactFormButtonText { get; set; }
    public string? ContactFormNote { get; set; }
    public string? ContactReachUsTitle { get; set; }
    public string? ContactFindUsTitle { get; set; }
    public string? ContactFindUsDescription { get; set; }
    // FAQs page specific fields
    public string? FaqHelpCardTitle { get; set; }
    public string? FaqHelpCardDescription { get; set; }
    public string? FaqHelpCardButtonText { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdatePageDto
{
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Eyebrow { get; set; }
    public string? HeroTitle { get; set; }
    public string? HeroText { get; set; }
    public string? Content { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? LastUpdated { get; set; }
    // About page specific fields
    public string? StatsJson { get; set; }
    public string? OurStoryTitle { get; set; }
    public string? OurStoryContent { get; set; }
    public string? HowWeWorkTitle { get; set; }
    public string? HowWeWorkContent { get; set; }
    public string? HowWeWorkChecklistJson { get; set; }
    public string? MissionEyebrow { get; set; }
    public string? MissionTitle { get; set; }
    public string? MissionCardsJson { get; set; }
    public string? WhoWeServeTitle { get; set; }
    public string? WhoWeServeContent { get; set; }
    public string? WhoWeServeCategoriesJson { get; set; }
    public string? WhatWeStandForTitle { get; set; }
    public string? ValuesJson { get; set; }
    public string? TeamSectionTitle { get; set; }
    public string? TeamSectionSubtitle { get; set; }
    public string? TeamMembersJson { get; set; }
    // Home page specific fields
    public string? HeroTrustBadgesJson { get; set; }
    public string? HeroMetricsJson { get; set; }
    public string? MiniEnquiryBadge { get; set; }
    public string? MiniEnquiryTitle { get; set; }
    public string? MiniEnquiryDescription { get; set; }
    public string? MiniEnquiryButtonText { get; set; }
    public string? MiniEnquiryNote { get; set; }
    public string? StatsBannerJson { get; set; }
    public string? CtaBannerJson { get; set; }
    public string? HowItWorksJson { get; set; }
    public string? WhySectionJson { get; set; }
    public string? PricingPlansJson { get; set; }
    public string? TrustSectionJson { get; set; }
    public string? TestimonialsJson { get; set; }
    public string? ValuePropositionJson { get; set; }
    // Contact page specific fields
    public string? ContactFormTitle { get; set; }
    public string? ContactFormDescription { get; set; }
    public string? ContactFormButtonText { get; set; }
    public string? ContactFormNote { get; set; }
    public string? ContactReachUsTitle { get; set; }
    public string? ContactFindUsTitle { get; set; }
    public string? ContactFindUsDescription { get; set; }
    // FAQs page specific fields
    public string? FaqHelpCardTitle { get; set; }
    public string? FaqHelpCardDescription { get; set; }
    public string? FaqHelpCardButtonText { get; set; }
    public bool? IsActive { get; set; }
}
