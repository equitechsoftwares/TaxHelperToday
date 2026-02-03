using System.ComponentModel.DataAnnotations;
using TaxHelperToday.Modules.Identity.Domain.Entities;

namespace TaxHelperToday.Modules.Admin.Domain.Entities;

public class AdminSetting
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;
    
    public string? Value { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public long? UpdatedBy { get; set; }

    // Navigation properties
    public User? UpdatedByUser { get; set; }
}
