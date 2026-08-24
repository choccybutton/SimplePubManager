using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Shift entity.
    /// </summary>
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        /// <summary>
        /// Configures the Shift entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            // Key configuration
            builder.HasKey(s => s.Id);

            // Property configurations
            builder.Property(s => s.OrganizationId)
                .IsRequired();

            builder.Property(s => s.StaffId)
                .IsRequired();

            builder.Property(s => s.CreatedBy)
                .IsRequired();

            builder.Property(s => s.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.StartTime)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(s => s.Organization)
                .WithMany(o => o.Shifts)
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Staff)
                .WithMany(u => u.Shifts)
                .HasForeignKey(s => s.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations for frequently queried columns
            builder.HasIndex(s => new { s.OrganizationId, s.Status });
            builder.HasIndex(s => new { s.OrganizationId, s.StartTime });
            builder.HasIndex(s => s.StaffId);
            builder.HasIndex(s => s.Status);

            // Relationship configurations
            builder.HasMany(s => s.Areas)
                .WithOne(sa => sa.Shift)
                .HasForeignKey(sa => sa.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.TimeLogs)
                .WithOne(sl => sl.Shift)
                .HasForeignKey(sl => sl.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Payment)
                .WithOne(sp => sp.Shift)
                .HasForeignKey<ShiftPayment>(sp => sp.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
