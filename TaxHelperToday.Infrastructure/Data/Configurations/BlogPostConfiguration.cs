using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("blog_posts");

        builder.HasKey(bp => bp.Id);
        builder.Property(bp => bp.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(bp => bp.Slug)
            .HasColumnName("slug")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(bp => bp.Slug).IsUnique();

        builder.Property(bp => bp.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(bp => bp.Excerpt)
            .HasColumnName("excerpt");

        builder.Property(bp => bp.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(bp => bp.Category)
            .HasColumnName("category")
            .HasMaxLength(100);

        builder.Property(bp => bp.ReadTime)
            .HasColumnName("read_time")
            .HasMaxLength(50);

        builder.Property(bp => bp.FeaturedImageUrl)
            .HasColumnName("featured_image_url")
            .HasMaxLength(500);

        builder.Property(bp => bp.MetaDescription)
            .HasColumnName("meta_description")
            .HasMaxLength(500);

        builder.Property(bp => bp.MetaKeywords)
            .HasColumnName("meta_keywords")
            .HasMaxLength(500);

        builder.Property(bp => bp.IsPublished)
            .HasColumnName("is_published")
            .HasDefaultValue(false);

        builder.Property(bp => bp.PublishedAt)
            .HasColumnName("published_at");

        builder.Property(bp => bp.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(bp => bp.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(bp => bp.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(bp => bp.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(bp => bp.ViewCount)
            .HasColumnName("view_count")
            .HasDefaultValue(0);

        builder.HasOne(bp => bp.Creator)
            .WithMany()
            .HasForeignKey(bp => bp.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(bp => bp.Updater)
            .WithMany()
            .HasForeignKey(bp => bp.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(bp => bp.Category);
        builder.HasIndex(bp => bp.IsPublished);
        builder.HasIndex(bp => bp.PublishedAt);
    }
}
