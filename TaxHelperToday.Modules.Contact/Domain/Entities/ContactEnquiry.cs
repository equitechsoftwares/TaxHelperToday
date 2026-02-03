using System.ComponentModel.DataAnnotations;
using TaxHelperToday.Modules.Content.Domain.Entities;
using TaxHelperToday.Modules.Identity.Domain.Entities;

namespace TaxHelperToday.Modules.Contact.Domain.Entities;

public class ContactEnquiry
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(500)]
    public string? Subject { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? EnquiryType { get; set; }
    
    public long? ServiceId { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";
    
    public long? AssignedTo { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Service? Service { get; set; }
    public User? AssignedUser { get; set; }
}
