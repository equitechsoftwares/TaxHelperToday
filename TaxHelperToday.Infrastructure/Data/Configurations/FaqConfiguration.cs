using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Content.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.ToTable("faqs");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(f => f.Question)
            .HasColumnName("question")
            .IsRequired();

        builder.Property(f => f.Answer)
            .HasColumnName("answer")
            .IsRequired();

        builder.Property(f => f.Category)
            .HasColumnName("category")
            .HasMaxLength(100);

        builder.Property(f => f.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0);

        builder.Property(f => f.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(f => f.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(f => f.Category);
        builder.HasIndex(f => f.IsActive);
        builder.HasIndex(f => f.DisplayOrder);
    }
}
