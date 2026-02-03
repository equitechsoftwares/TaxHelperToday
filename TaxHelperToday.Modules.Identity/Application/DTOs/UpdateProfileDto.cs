using System.ComponentModel.DataAnnotations;

namespace TaxHelperToday.Modules.Identity.Application.DTOs;

public class UpdateProfileDto
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}
