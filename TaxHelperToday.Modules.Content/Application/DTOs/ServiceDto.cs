namespace TaxHelperToday.Modules.Content.Application.DTOs;

public class ServiceDto
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? Level { get; set; }
    public string? Highlight { get; set; }
    public string? IconUrl { get; set; }
    // Enquiry section content
    public string? EnquiryTitle { get; set; }
    public string? EnquirySubtitle { get; set; }
    public string? EnquiryButtonText { get; set; }
    public string? EnquiryNote { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateServiceDto
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? Level { get; set; }
    public string? Highlight { get; set; }
    public string? IconUrl { get; set; }
    // Enquiry section content
    public string? EnquiryTitle { get; set; }
    public string? EnquirySubtitle { get; set; }
    public string? EnquiryButtonText { get; set; }
    public string? EnquiryNote { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateServiceDto
{
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? Level { get; set; }
    public string? Highlight { get; set; }
    public string? IconUrl { get; set; }
    // Enquiry section content
    public string? EnquiryTitle { get; set; }
    public string? EnquirySubtitle { get; set; }
    public string? EnquiryButtonText { get; set; }
    public string? EnquiryNote { get; set; }
    public bool? IsActive { get; set; }
    public int? DisplayOrder { get; set; }
}
