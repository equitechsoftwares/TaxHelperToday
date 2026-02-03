using System.ComponentModel.DataAnnotations;
using TaxHelperToday.Modules.Identity.Domain.Entities;

namespace TaxHelperToday.Modules.Content.Domain.Entities;

public class BlogPost
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    public string? Excerpt { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(50)]
    public string? ReadTime { get; set; }
    
    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }
    
    [MaxLength(500)]
    public string? MetaDescription { get; set; }
    
    [MaxLength(500)]
    public string? MetaKeywords { get; set; }
    
    public bool IsPublished { get; set; } = false;
    
    public DateTime? PublishedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int ViewCount { get; set; } = 0;

    // Navigation properties
    public User? Creator { get; set; }
    public User? Updater { get; set; }
    public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
}
