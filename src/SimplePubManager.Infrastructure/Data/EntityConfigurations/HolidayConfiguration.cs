using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Holiday entity.
    /// </summary>
    public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
    {
        /// <summary>
        /// Configures the Holiday entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            // Key configuration
            builder.HasKey(h => h.Id);

            // Property configurations
            builder.Property(h => h.StaffId)
                .IsRequired();

            builder.Property(h => h.OrganizationId)
                .IsRequired();

            builder.Property(h => h.StartDate)
                .IsRequired();

            builder.Property(h => h.EndDate)
                .IsRequired();

            builder.Property(h => h.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(h => h.RequestedAt)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(h => h.Staff)
                .WithMany(u => u.Holidays)
                .HasForeignKey(h => h.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.Organization)
                .WithMany(o => o.Holidays)
                .HasForeignKey(h => h.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.ApprovedByUser)
                .WithMany()
                .HasForeignKey(h => h.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Index configurations
            builder.HasIndex(h => h.StaffId);
            builder.HasIndex(h => h.OrganizationId);
            builder.HasIndex(h => h.Status);
            builder.HasIndex(h => new { h.StaffId, h.StartDate });
        }
    }
}
