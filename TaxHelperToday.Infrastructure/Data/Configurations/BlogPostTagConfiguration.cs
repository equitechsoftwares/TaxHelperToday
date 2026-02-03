using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class BlogPostTagConfiguration : IEntityTypeConfiguration<BlogPostTag>
{
    public void Configure(EntityTypeBuilder<BlogPostTag> builder)
    {
        builder.ToTable("blog_post_tags");

        builder.HasKey(bpt => new { bpt.BlogPostId, bpt.TagId });

        builder.Property(bpt => bpt.BlogPostId)
            .HasColumnName("blog_post_id");

        builder.Property(bpt => bpt.TagId)
            .HasColumnName("tag_id");

        builder.HasOne(bpt => bpt.BlogPost)
            .WithMany(bp => bp.BlogPostTags)
            .HasForeignKey(bpt => bpt.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bpt => bpt.Tag)
            .WithMany(bt => bt.BlogPostTags)
            .HasForeignKey(bpt => bpt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
