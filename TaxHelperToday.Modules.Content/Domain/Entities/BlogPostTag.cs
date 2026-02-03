namespace TaxHelperToday.Modules.Content.Domain.Entities;

public class BlogPostTag
{
    public long BlogPostId { get; set; }
    public long TagId { get; set; }

    // Navigation properties
    public BlogPost BlogPost { get; set; } = null!;
    public BlogTag Tag { get; set; } = null!;
}
