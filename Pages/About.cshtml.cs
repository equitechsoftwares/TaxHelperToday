using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Content.Application.Services;
using System.Text.Json;

namespace TaxHelperToday.Pages
{
    public class AboutModel : PageModel
    {
        private readonly IPageService _pageService;

        public AboutModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        public string? Eyebrow { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroText { get; set; }
        public List<StatItem>? Stats { get; set; }
        public string? OurStoryTitle { get; set; }
        public string? OurStoryContent { get; set; }
        public string? HowWeWorkTitle { get; set; }
        public string? HowWeWorkContent { get; set; }
        public List<string>? HowWeWorkChecklist { get; set; }
        public string? MissionEyebrow { get; set; }
        public string? MissionTitle { get; set; }
        public List<MissionCard>? MissionCards { get; set; }
        public string? WhoWeServeTitle { get; set; }
        public string? WhoWeServeContent { get; set; }
        public List<CategoryBadge>? WhoWeServeCategories { get; set; }
        public string? WhatWeStandForTitle { get; set; }
        public List<ValuePill>? Values { get; set; }
        public string? TeamSectionTitle { get; set; }
        public string? TeamSectionSubtitle { get; set; }
        public List<TeamMember>? TeamMembers { get; set; }
        public string Title { get; set; } = "About";

        public async Task OnGetAsync()
        {
            var page = await _pageService.GetBySlugAsync("about");
            if (page != null)
            {
                Title = page.Title;
                Eyebrow = page.Eyebrow;
                HeroTitle = page.HeroTitle;
                HeroText = page.HeroText;
                OurStoryTitle = page.OurStoryTitle;
                OurStoryContent = page.OurStoryContent;
                HowWeWorkTitle = page.HowWeWorkTitle;
                HowWeWorkContent = page.HowWeWorkContent;
                MissionEyebrow = page.MissionEyebrow;
                MissionTitle = page.MissionTitle;
                WhoWeServeTitle = page.WhoWeServeTitle;
                WhoWeServeContent = page.WhoWeServeContent;
                WhatWeStandForTitle = page.WhatWeStandForTitle;
                TeamSectionTitle = page.TeamSectionTitle;
                TeamSectionSubtitle = page.TeamSectionSubtitle;

                // Parse JSON fields with case-insensitive property matching
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (!string.IsNullOrEmpty(page.StatsJson))
                {
                    Stats = JsonSerializer.Deserialize<List<StatItem>>(page.StatsJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.HowWeWorkChecklistJson))
                {
                    HowWeWorkChecklist = JsonSerializer.Deserialize<List<string>>(page.HowWeWorkChecklistJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.MissionCardsJson))
                {
                    MissionCards = JsonSerializer.Deserialize<List<MissionCard>>(page.MissionCardsJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.WhoWeServeCategoriesJson))
                {
                    WhoWeServeCategories = JsonSerializer.Deserialize<List<CategoryBadge>>(page.WhoWeServeCategoriesJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.ValuesJson))
                {
                    Values = JsonSerializer.Deserialize<List<ValuePill>>(page.ValuesJson, jsonOptions);
                }

                if (!string.IsNullOrEmpty(page.TeamMembersJson))
                {
                    TeamMembers = JsonSerializer.Deserialize<List<TeamMember>>(page.TeamMembersJson, jsonOptions);
                }
            }
        }
    }

    public class StatItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MissionCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CategoryBadge
    {
        public string Icon { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class ValuePill
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }

    public class TeamMember
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
