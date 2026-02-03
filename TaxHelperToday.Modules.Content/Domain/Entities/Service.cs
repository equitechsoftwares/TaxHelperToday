using System.ComponentModel.DataAnnotations;

namespace TaxHelperToday.Modules.Content.Domain.Entities;

public class Service
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? Content { get; set; }
    
    [MaxLength(100)]
    public string? Type { get; set; }
    
    [MaxLength(50)]
    public string? Level { get; set; }
    
    public string? Highlight { get; set; }
    
    [MaxLength(500)]
    public string? IconUrl { get; set; }
    
    // Enquiry section content
    [MaxLength(200)]
    public string? EnquiryTitle { get; set; } // Title for service enquiry section
    public string? EnquirySubtitle { get; set; } // Subtitle/description for service enquiry section
    [MaxLength(100)]
    public string? EnquiryButtonText { get; set; } // Button text for service enquiry form
    [MaxLength(500)]
    public string? EnquiryNote { get; set; } // Note text below service enquiry form
    
    public bool IsActive { get; set; } = true;
    
    public int DisplayOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
