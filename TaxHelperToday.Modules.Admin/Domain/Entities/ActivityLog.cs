using System.ComponentModel.DataAnnotations;
using TaxHelperToday.Modules.Identity.Domain.Entities;
using System.Text.Json;

namespace TaxHelperToday.Modules.Admin.Domain.Entities;

public class ActivityLog
{
    public long Id { get; set; }
    
    public long? UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? EntityType { get; set; }
    
    public long? EntityId { get; set; }
    
    public JsonDocument? Details { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
}
