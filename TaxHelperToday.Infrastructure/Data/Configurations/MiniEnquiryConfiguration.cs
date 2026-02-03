using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class MiniEnquiryConfiguration : IEntityTypeConfiguration<MiniEnquiry>
{
    public void Configure(EntityTypeBuilder<MiniEnquiry> builder)
    {
        builder.ToTable("mini_enquiries");

        builder.HasKey(me => me.Id);
        builder.Property(me => me.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(me => me.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(me => me.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(me => me.UserType)
            .HasColumnName("user_type")
            .HasMaxLength(50);

        builder.Property(me => me.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasDefaultValue("New");

        builder.Property(me => me.AssignedTo)
            .HasColumnName("assigned_to");

        builder.Property(me => me.Notes)
            .HasColumnName("notes");

        builder.Property(me => me.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(me => me.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(me => me.AssignedUser)
            .WithMany()
            .HasForeignKey(me => me.AssignedTo)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(me => me.Status);
        builder.HasIndex(me => me.CreatedAt);
    }
}
