using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the ShiftPayment entity.
    /// </summary>
    public class ShiftPaymentConfiguration : IEntityTypeConfiguration<ShiftPayment>
    {
        /// <summary>
        /// Configures the ShiftPayment entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<ShiftPayment> builder)
        {
            // Key configuration
            builder.HasKey(sp => sp.Id);

            // Property configurations
            builder.Property(sp => sp.ShiftId)
                .IsRequired();

            builder.Property(sp => sp.HourlyRate)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(sp => sp.HoursWorked)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(sp => sp.Amount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(sp => sp.Status)
                .IsRequired()
                .HasConversion<int>();

            // Foreign key configuration - one-to-one relationship
            builder.HasOne(sp => sp.Shift)
                .WithOne(s => s.Payment)
                .HasForeignKey<ShiftPayment>(sp => sp.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations
            builder.HasIndex(sp => sp.ShiftId);
            builder.HasIndex(sp => sp.Status);
        }
    }
}
