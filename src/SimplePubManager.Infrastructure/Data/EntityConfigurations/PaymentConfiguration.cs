using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Payment entity.
    /// </summary>
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        /// <summary>
        /// Configures the Payment entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Key configuration
            builder.HasKey(p => p.Id);

            // Property configurations
            builder.Property(p => p.OrganizationId)
                .IsRequired();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Date)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(p => p.Organization)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Staff)
                .WithMany()
                .HasForeignKey(p => p.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.RelatedShift)
                .WithMany()
                .HasForeignKey(p => p.RelatedShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index configurations
            builder.HasIndex(p => p.OrganizationId);
            builder.HasIndex(p => p.StaffId);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Date);
        }
    }
}
