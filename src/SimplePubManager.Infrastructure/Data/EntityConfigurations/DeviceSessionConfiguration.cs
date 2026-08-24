using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the DeviceSession entity.
    /// </summary>
    public class DeviceSessionConfiguration : IEntityTypeConfiguration<DeviceSession>
    {
        /// <summary>
        /// Configures the DeviceSession entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<DeviceSession> builder)
        {
            // Key configuration
            builder.HasKey(ds => ds.Id);

            // Property configurations
            builder.Property(ds => ds.DeviceId)
                .IsRequired();

            builder.Property(ds => ds.UserId)
                .IsRequired();

            builder.Property(ds => ds.SessionToken)
                .IsRequired();

            builder.Property(ds => ds.CreatedAt)
                .IsRequired();

            builder.Property(ds => ds.ExpiresAt)
                .IsRequired();

            builder.Property(ds => ds.LastActivityAt)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(ds => ds.Device)
                .WithMany(d => d.Sessions)
                .HasForeignKey(ds => ds.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ds => ds.User)
                .WithMany(u => u.DeviceSessions)
                .HasForeignKey(ds => ds.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations
            builder.HasIndex(ds => ds.DeviceId);
            builder.HasIndex(ds => ds.UserId);
            builder.HasIndex(ds => ds.ExpiresAt);
            builder.HasIndex(ds => ds.LastActivityAt);
        }
    }
}
