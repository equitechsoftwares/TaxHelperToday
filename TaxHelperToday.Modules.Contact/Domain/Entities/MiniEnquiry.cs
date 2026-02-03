using System.ComponentModel.DataAnnotations;
using TaxHelperToday.Modules.Identity.Domain.Entities;

namespace TaxHelperToday.Modules.Contact.Domain.Entities;

public class MiniEnquiry
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? UserType { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "New";
    
    public long? AssignedTo { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? AssignedUser { get; set; }
}
