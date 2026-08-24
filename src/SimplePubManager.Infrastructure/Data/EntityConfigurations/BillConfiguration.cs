using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Bill entity.
    /// </summary>
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        /// <summary>
        /// Configures the Bill entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            // Key configuration
            builder.HasKey(b => b.Id);

            // Property configurations
            builder.Property(b => b.OrganizationId)
                .IsRequired();

            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Amount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(b => b.DueDate)
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<int>();

            // Foreign key configuration
            builder.HasOne(b => b.Organization)
                .WithMany(o => o.Bills)
                .HasForeignKey(b => b.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations
            builder.HasIndex(b => b.OrganizationId);
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.DueDate);
        }
    }
}
