using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(s => s.Slug)
            .HasColumnName("slug")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(s => s.Slug).IsUnique();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasColumnName("description");

        builder.Property(s => s.Content)
            .HasColumnName("content");

        builder.Property(s => s.Type)
            .HasColumnName("type")
            .HasMaxLength(100);

        builder.Property(s => s.Level)
            .HasColumnName("level")
            .HasMaxLength(50);

        builder.Property(s => s.Highlight)
            .HasColumnName("highlight");

        builder.Property(s => s.IconUrl)
            .HasColumnName("icon_url")
            .HasMaxLength(500);

        // Enquiry section content
        builder.Property(s => s.EnquiryTitle)
            .HasColumnName("enquiry_title")
            .HasMaxLength(200);

        builder.Property(s => s.EnquirySubtitle)
            .HasColumnName("enquiry_subtitle");

        builder.Property(s => s.EnquiryButtonText)
            .HasColumnName("enquiry_button_text")
            .HasMaxLength(100);

        builder.Property(s => s.EnquiryNote)
            .HasColumnName("enquiry_note")
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(s => s.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.DisplayOrder);
    }
}
