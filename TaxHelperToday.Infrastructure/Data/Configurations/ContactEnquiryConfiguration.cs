using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxHelperToday.Modules.Contact.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data.Configurations;

public class ContactEnquiryConfiguration : IEntityTypeConfiguration<ContactEnquiry>
{
    public void Configure(EntityTypeBuilder<ContactEnquiry> builder)
    {
        builder.ToTable("contact_enquiries");

        builder.HasKey(ce => ce.Id);
        builder.Property(ce => ce.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .UseIdentityByDefaultColumn();

        builder.Property(ce => ce.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ce => ce.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ce => ce.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(ce => ce.Subject)
            .HasColumnName("subject")
            .HasMaxLength(500);

        builder.Property(ce => ce.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.Property(ce => ce.EnquiryType)
            .HasColumnName("enquiry_type")
            .HasMaxLength(50);

        builder.Property(ce => ce.ServiceId)
            .HasColumnName("service_id");

        builder.Property(ce => ce.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(ce => ce.AssignedTo)
            .HasColumnName("assigned_to");

        builder.Property(ce => ce.Notes)
            .HasColumnName("notes");

        builder.Property(ce => ce.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ce => ce.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(ce => ce.Service)
            .WithMany()
            .HasForeignKey(ce => ce.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ce => ce.AssignedUser)
            .WithMany()
            .HasForeignKey(ce => ce.AssignedTo)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(ce => ce.Status);
        builder.HasIndex(ce => ce.EnquiryType);
        builder.HasIndex(ce => ce.CreatedAt);
        builder.HasIndex(ce => ce.AssignedTo);
    }
}
