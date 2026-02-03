namespace TaxHelperToday.Modules.Content.Application.DTOs;

public class BlogPostDto
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ReadTime { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class CreateBlogPostDto
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ReadTime { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsPublished { get; set; } = false;
    public List<string> Tags { get; set; } = new();
}

public class UpdateBlogPostDto
{
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? ReadTime { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool? IsPublished { get; set; }
    public List<string>? Tags { get; set; }
}
