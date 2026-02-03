using System.ComponentModel.DataAnnotations;

namespace TaxHelperToday.Modules.Identity.Domain.Entities;

public class User
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsEmailVerified { get; set; } = false;
    
    [MaxLength(500)]
    public string? EmailVerificationToken { get; set; }
    
    [MaxLength(500)]
    public string? PasswordResetToken { get; set; }
    
    public DateTime? PasswordResetExpires { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Token version - incremented when all sessions are revoked to invalidate all existing tokens
    /// </summary>
    public long TokenVersion { get; set; } = 0;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
