using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("pages");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(p => p.Slug)
            .HasColumnName("slug")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Eyebrow)
            .HasColumnName("eyebrow")
            .HasMaxLength(200);

        builder.Property(p => p.HeroTitle)
            .HasColumnName("hero_title")
            .HasMaxLength(500);

        builder.Property(p => p.HeroText)
            .HasColumnName("hero_text");

        builder.Property(p => p.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(p => p.MetaDescription)
            .HasColumnName("meta_description")
            .HasMaxLength(500);

        builder.Property(p => p.MetaKeywords)
            .HasColumnName("meta_keywords")
            .HasMaxLength(500);

        builder.Property(p => p.LastUpdated)
            .HasColumnName("last_updated")
            .HasMaxLength(100);

        // About page specific fields
        builder.Property(p => p.StatsJson)
            .HasColumnName("stats_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.OurStoryTitle)
            .HasColumnName("our_story_title")
            .HasMaxLength(200);

        builder.Property(p => p.OurStoryContent)
            .HasColumnName("our_story_content");

        builder.Property(p => p.HowWeWorkTitle)
            .HasColumnName("how_we_work_title")
            .HasMaxLength(200);

        builder.Property(p => p.HowWeWorkContent)
            .HasColumnName("how_we_work_content");

        builder.Property(p => p.HowWeWorkChecklistJson)
            .HasColumnName("how_we_work_checklist_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.MissionEyebrow)
            .HasColumnName("mission_eyebrow")
            .HasMaxLength(200);

        builder.Property(p => p.MissionTitle)
            .HasColumnName("mission_title")
            .HasMaxLength(500);

        builder.Property(p => p.MissionCardsJson)
            .HasColumnName("mission_cards_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.WhoWeServeTitle)
            .HasColumnName("who_we_serve_title")
            .HasMaxLength(200);

        builder.Property(p => p.WhoWeServeContent)
            .HasColumnName("who_we_serve_content");

        builder.Property(p => p.WhoWeServeCategoriesJson)
            .HasColumnName("who_we_serve_categories_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.WhatWeStandForTitle)
            .HasColumnName("what_we_stand_for_title")
            .HasMaxLength(200);

        builder.Property(p => p.ValuesJson)
            .HasColumnName("values_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.TeamSectionTitle)
            .HasColumnName("team_section_title")
            .HasMaxLength(200);

        builder.Property(p => p.TeamSectionSubtitle)
            .HasColumnName("team_section_subtitle")
            .HasMaxLength(500);

        builder.Property(p => p.TeamMembersJson)
            .HasColumnName("team_members_json")
            .HasColumnType("jsonb");

        // Home page specific fields
        builder.Property(p => p.HeroTrustBadgesJson)
            .HasColumnName("hero_trust_badges_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.HeroMetricsJson)
            .HasColumnName("hero_metrics_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.MiniEnquiryBadge)
            .HasColumnName("mini_enquiry_badge")
            .HasMaxLength(100);

        builder.Property(p => p.MiniEnquiryTitle)
            .HasColumnName("mini_enquiry_title")
            .HasMaxLength(200);

        builder.Property(p => p.MiniEnquiryDescription)
            .HasColumnName("mini_enquiry_description");

        builder.Property(p => p.MiniEnquiryButtonText)
            .HasColumnName("mini_enquiry_button_text")
            .HasMaxLength(100);

        builder.Property(p => p.MiniEnquiryNote)
            .HasColumnName("mini_enquiry_note")
            .HasMaxLength(500);

        builder.Property(p => p.StatsBannerJson)
            .HasColumnName("stats_banner_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.CtaBannerJson)
            .HasColumnName("cta_banner_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.HowItWorksJson)
            .HasColumnName("how_it_works_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.WhySectionJson)
            .HasColumnName("why_section_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.PricingPlansJson)
            .HasColumnName("pricing_plans_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.TrustSectionJson)
            .HasColumnName("trust_section_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.TestimonialsJson)
            .HasColumnName("testimonials_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.ValuePropositionJson)
            .HasColumnName("value_proposition_json")
            .HasColumnType("jsonb");

        // Contact page specific fields
        builder.Property(p => p.ContactFormTitle)
            .HasColumnName("contact_form_title")
            .HasMaxLength(200);

        builder.Property(p => p.ContactFormDescription)
            .HasColumnName("contact_form_description");

        builder.Property(p => p.ContactFormButtonText)
            .HasColumnName("contact_form_button_text")
            .HasMaxLength(100);

        builder.Property(p => p.ContactFormNote)
            .HasColumnName("contact_form_note")
            .HasMaxLength(500);

        builder.Property(p => p.ContactReachUsTitle)
            .HasColumnName("contact_reach_us_title")
            .HasMaxLength(200);

        builder.Property(p => p.ContactFindUsTitle)
            .HasColumnName("contact_find_us_title")
            .HasMaxLength(200);

        builder.Property(p => p.ContactFindUsDescription)
            .HasColumnName("contact_find_us_description");

        // FAQs page specific fields
        builder.Property(p => p.FaqHelpCardTitle)
            .HasColumnName("faq_help_card_title")
            .HasMaxLength(200);

        builder.Property(p => p.FaqHelpCardDescription)
            .HasColumnName("faq_help_card_description");

        builder.Property(p => p.FaqHelpCardButtonText)
            .HasColumnName("faq_help_card_button_text")
            .HasMaxLength(100);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(p => p.IsActive);
    }
}
