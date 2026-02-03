using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Content.Application.DTOs;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Modules.Content.Infrastructure.Services;

public class PageService : IPageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PageService> _logger;

    public PageService(ApplicationDbContext context, ILogger<PageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PageDto?> GetByIdAsync(long id)
    {
        var page = await _context.Pages.FindAsync(id);
        return page == null ? null : MapToDto(page);
    }

    public async Task<PageDto?> GetBySlugAsync(string slug, bool activeOnly = true)
    {
        var query = _context.Pages.Where(p => p.Slug == slug);
        
        if (activeOnly)
        {
            query = query.Where(p => p.IsActive);
        }
        
        var page = await query.FirstOrDefaultAsync();
        return page == null ? null : MapToDto(page);
    }

    public async Task<IEnumerable<PageDto>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.Pages.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(p => p.IsActive);
        }

        var pages = await query
            .OrderBy(p => p.Title)
            .ToListAsync();

        return pages.Select(MapToDto);
    }

    public async Task<PageDto> CreateAsync(CreatePageDto dto)
    {
        var page = new Page
        {
            Slug = dto.Slug,
            Title = dto.Title,
            Eyebrow = dto.Eyebrow,
            HeroTitle = dto.HeroTitle,
            HeroText = dto.HeroText,
            Content = dto.Content,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            LastUpdated = dto.LastUpdated,
            StatsJson = dto.StatsJson,
            OurStoryTitle = dto.OurStoryTitle,
            OurStoryContent = dto.OurStoryContent,
            HowWeWorkTitle = dto.HowWeWorkTitle,
            HowWeWorkContent = dto.HowWeWorkContent,
            HowWeWorkChecklistJson = dto.HowWeWorkChecklistJson,
            MissionEyebrow = dto.MissionEyebrow,
            MissionTitle = dto.MissionTitle,
            MissionCardsJson = dto.MissionCardsJson,
            WhoWeServeTitle = dto.WhoWeServeTitle,
            WhoWeServeContent = dto.WhoWeServeContent,
            WhoWeServeCategoriesJson = dto.WhoWeServeCategoriesJson,
            WhatWeStandForTitle = dto.WhatWeStandForTitle,
            ValuesJson = dto.ValuesJson,
            TeamSectionTitle = dto.TeamSectionTitle,
            TeamSectionSubtitle = dto.TeamSectionSubtitle,
            TeamMembersJson = dto.TeamMembersJson,
            HeroTrustBadgesJson = dto.HeroTrustBadgesJson,
            HeroMetricsJson = dto.HeroMetricsJson,
            MiniEnquiryBadge = dto.MiniEnquiryBadge,
            MiniEnquiryTitle = dto.MiniEnquiryTitle,
            MiniEnquiryDescription = dto.MiniEnquiryDescription,
            MiniEnquiryButtonText = dto.MiniEnquiryButtonText,
            MiniEnquiryNote = dto.MiniEnquiryNote,
            StatsBannerJson = dto.StatsBannerJson,
            CtaBannerJson = dto.CtaBannerJson,
            HowItWorksJson = dto.HowItWorksJson,
            WhySectionJson = dto.WhySectionJson,
            PricingPlansJson = dto.PricingPlansJson,
            TrustSectionJson = dto.TrustSectionJson,
            TestimonialsJson = dto.TestimonialsJson,
            ValuePropositionJson = dto.ValuePropositionJson,
            ContactFormTitle = dto.ContactFormTitle,
            ContactFormDescription = dto.ContactFormDescription,
            ContactFormButtonText = dto.ContactFormButtonText,
            ContactFormNote = dto.ContactFormNote,
            ContactReachUsTitle = dto.ContactReachUsTitle,
            ContactFindUsTitle = dto.ContactFindUsTitle,
            ContactFindUsDescription = dto.ContactFindUsDescription,
            FaqHelpCardTitle = dto.FaqHelpCardTitle,
            FaqHelpCardDescription = dto.FaqHelpCardDescription,
            FaqHelpCardButtonText = dto.FaqHelpCardButtonText,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Pages.Add(page);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(page.Id))!;
    }

    public async Task<PageDto> UpdateAsync(long id, UpdatePageDto dto)
    {
        var page = await _context.Pages.FindAsync(id);
        if (page == null)
        {
            throw new KeyNotFoundException($"Page with id {id} not found");
        }

        if (dto.Slug != null) page.Slug = dto.Slug;
        if (dto.Title != null) page.Title = dto.Title;
        if (dto.Eyebrow != null) page.Eyebrow = dto.Eyebrow;
        if (dto.HeroTitle != null) page.HeroTitle = dto.HeroTitle;
        if (dto.HeroText != null) page.HeroText = dto.HeroText;
        if (dto.Content != null) page.Content = dto.Content;
        if (dto.MetaDescription != null) page.MetaDescription = dto.MetaDescription;
        if (dto.MetaKeywords != null) page.MetaKeywords = dto.MetaKeywords;
        if (dto.LastUpdated != null) page.LastUpdated = dto.LastUpdated;
        if (dto.StatsJson != null) page.StatsJson = dto.StatsJson;
        if (dto.OurStoryTitle != null) page.OurStoryTitle = dto.OurStoryTitle;
        if (dto.OurStoryContent != null) page.OurStoryContent = dto.OurStoryContent;
        if (dto.HowWeWorkTitle != null) page.HowWeWorkTitle = dto.HowWeWorkTitle;
        if (dto.HowWeWorkContent != null) page.HowWeWorkContent = dto.HowWeWorkContent;
        if (dto.HowWeWorkChecklistJson != null) page.HowWeWorkChecklistJson = dto.HowWeWorkChecklistJson;
        if (dto.MissionEyebrow != null) page.MissionEyebrow = dto.MissionEyebrow;
        if (dto.MissionTitle != null) page.MissionTitle = dto.MissionTitle;
        if (dto.MissionCardsJson != null) page.MissionCardsJson = dto.MissionCardsJson;
        if (dto.WhoWeServeTitle != null) page.WhoWeServeTitle = dto.WhoWeServeTitle;
        if (dto.WhoWeServeContent != null) page.WhoWeServeContent = dto.WhoWeServeContent;
        if (dto.WhoWeServeCategoriesJson != null) page.WhoWeServeCategoriesJson = dto.WhoWeServeCategoriesJson;
        if (dto.WhatWeStandForTitle != null) page.WhatWeStandForTitle = dto.WhatWeStandForTitle;
        if (dto.ValuesJson != null) page.ValuesJson = dto.ValuesJson;
        if (dto.TeamSectionTitle != null) page.TeamSectionTitle = dto.TeamSectionTitle;
        if (dto.TeamSectionSubtitle != null) page.TeamSectionSubtitle = dto.TeamSectionSubtitle;
        if (dto.TeamMembersJson != null) page.TeamMembersJson = dto.TeamMembersJson;
        if (dto.HeroTrustBadgesJson != null) page.HeroTrustBadgesJson = dto.HeroTrustBadgesJson;
        if (dto.HeroMetricsJson != null) page.HeroMetricsJson = dto.HeroMetricsJson;
        if (dto.MiniEnquiryBadge != null) page.MiniEnquiryBadge = dto.MiniEnquiryBadge;
        if (dto.MiniEnquiryTitle != null) page.MiniEnquiryTitle = dto.MiniEnquiryTitle;
        if (dto.MiniEnquiryDescription != null) page.MiniEnquiryDescription = dto.MiniEnquiryDescription;
        if (dto.MiniEnquiryButtonText != null) page.MiniEnquiryButtonText = dto.MiniEnquiryButtonText;
        if (dto.MiniEnquiryNote != null) page.MiniEnquiryNote = dto.MiniEnquiryNote;
        if (dto.StatsBannerJson != null) page.StatsBannerJson = dto.StatsBannerJson;
        if (dto.CtaBannerJson != null) page.CtaBannerJson = dto.CtaBannerJson;
        if (dto.HowItWorksJson != null) page.HowItWorksJson = dto.HowItWorksJson;
        if (dto.WhySectionJson != null) page.WhySectionJson = dto.WhySectionJson;
        if (dto.PricingPlansJson != null) page.PricingPlansJson = dto.PricingPlansJson;
        if (dto.TrustSectionJson != null) page.TrustSectionJson = dto.TrustSectionJson;
        if (dto.TestimonialsJson != null) page.TestimonialsJson = dto.TestimonialsJson;
        if (dto.ValuePropositionJson != null) page.ValuePropositionJson = dto.ValuePropositionJson;
        if (dto.ContactFormTitle != null) page.ContactFormTitle = dto.ContactFormTitle;
        if (dto.ContactFormDescription != null) page.ContactFormDescription = dto.ContactFormDescription;
        if (dto.ContactFormButtonText != null) page.ContactFormButtonText = dto.ContactFormButtonText;
        if (dto.ContactFormNote != null) page.ContactFormNote = dto.ContactFormNote;
        if (dto.ContactReachUsTitle != null) page.ContactReachUsTitle = dto.ContactReachUsTitle;
        if (dto.ContactFindUsTitle != null) page.ContactFindUsTitle = dto.ContactFindUsTitle;
        if (dto.ContactFindUsDescription != null) page.ContactFindUsDescription = dto.ContactFindUsDescription;
        if (dto.FaqHelpCardTitle != null) page.FaqHelpCardTitle = dto.FaqHelpCardTitle;
        if (dto.FaqHelpCardDescription != null) page.FaqHelpCardDescription = dto.FaqHelpCardDescription;
        if (dto.FaqHelpCardButtonText != null) page.FaqHelpCardButtonText = dto.FaqHelpCardButtonText;
        if (dto.IsActive.HasValue) page.IsActive = dto.IsActive.Value;

        page.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var page = await _context.Pages.FindAsync(id);
        if (page == null)
        {
            return false;
        }

        _context.Pages.Remove(page);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(long id)
    {
        var page = await _context.Pages.FindAsync(id);
        if (page == null)
        {
            return false;
        }

        page.IsActive = !page.IsActive;
        page.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static PageDto MapToDto(Page page)
    {
        return new PageDto
        {
            Id = page.Id,
            Slug = page.Slug,
            Title = page.Title,
            Eyebrow = page.Eyebrow,
            HeroTitle = page.HeroTitle,
            HeroText = page.HeroText,
            Content = page.Content,
            MetaDescription = page.MetaDescription,
            MetaKeywords = page.MetaKeywords,
            LastUpdated = page.LastUpdated,
            StatsJson = page.StatsJson,
            OurStoryTitle = page.OurStoryTitle,
            OurStoryContent = page.OurStoryContent,
            HowWeWorkTitle = page.HowWeWorkTitle,
            HowWeWorkContent = page.HowWeWorkContent,
            HowWeWorkChecklistJson = page.HowWeWorkChecklistJson,
            MissionEyebrow = page.MissionEyebrow,
            MissionTitle = page.MissionTitle,
            MissionCardsJson = page.MissionCardsJson,
            WhoWeServeTitle = page.WhoWeServeTitle,
            WhoWeServeContent = page.WhoWeServeContent,
            WhoWeServeCategoriesJson = page.WhoWeServeCategoriesJson,
            WhatWeStandForTitle = page.WhatWeStandForTitle,
            ValuesJson = page.ValuesJson,
            TeamSectionTitle = page.TeamSectionTitle,
            TeamSectionSubtitle = page.TeamSectionSubtitle,
            TeamMembersJson = page.TeamMembersJson,
            HeroTrustBadgesJson = page.HeroTrustBadgesJson,
            HeroMetricsJson = page.HeroMetricsJson,
            MiniEnquiryBadge = page.MiniEnquiryBadge,
            MiniEnquiryTitle = page.MiniEnquiryTitle,
            MiniEnquiryDescription = page.MiniEnquiryDescription,
            MiniEnquiryButtonText = page.MiniEnquiryButtonText,
            MiniEnquiryNote = page.MiniEnquiryNote,
            StatsBannerJson = page.StatsBannerJson,
            CtaBannerJson = page.CtaBannerJson,
            HowItWorksJson = page.HowItWorksJson,
            WhySectionJson = page.WhySectionJson,
            PricingPlansJson = page.PricingPlansJson,
            TrustSectionJson = page.TrustSectionJson,
            TestimonialsJson = page.TestimonialsJson,
            ValuePropositionJson = page.ValuePropositionJson,
            ContactFormTitle = page.ContactFormTitle,
            ContactFormDescription = page.ContactFormDescription,
            ContactFormButtonText = page.ContactFormButtonText,
            ContactFormNote = page.ContactFormNote,
            ContactReachUsTitle = page.ContactReachUsTitle,
            ContactFindUsTitle = page.ContactFindUsTitle,
            ContactFindUsDescription = page.ContactFindUsDescription,
            FaqHelpCardTitle = page.FaqHelpCardTitle,
            FaqHelpCardDescription = page.FaqHelpCardDescription,
            FaqHelpCardButtonText = page.FaqHelpCardButtonText,
            IsActive = page.IsActive,
            CreatedAt = page.CreatedAt,
            UpdatedAt = page.UpdatedAt
        };
    }
}
