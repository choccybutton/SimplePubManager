using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Device entity.
    /// </summary>
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        /// <summary>
        /// Configures the Device entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            // Key configuration
            builder.HasKey(d => d.Id);

            // Property configurations
            builder.Property(d => d.OrganizationId)
                .IsRequired();

            builder.Property(d => d.DeviceId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.DeviceKeyHash)
                .IsRequired();

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.Enabled)
                .IsRequired();

            builder.Property(d => d.Location)
                .HasMaxLength(500);

            // Unique constraint for (OrganizationId, DeviceId)
            builder.HasIndex(d => new { d.OrganizationId, d.DeviceId })
                .IsUnique();

            // Individual indices
            builder.HasIndex(d => d.Enabled);

            // Foreign key configuration
            builder.HasOne(d => d.Organization)
                .WithMany(o => o.Devices)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship configurations
            builder.HasMany(d => d.Sessions)
                .WithOne(ds => ds.Device)
                .HasForeignKey(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
