using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the UserPin entity.
    /// </summary>
    public class UserPinConfiguration : IEntityTypeConfiguration<UserPin>
    {
        /// <summary>
        /// Configures the UserPin entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<UserPin> builder)
        {
            // Key configuration
            builder.HasKey(up => up.Id);

            // Property configurations
            builder.Property(up => up.UserId)
                .IsRequired();

            builder.Property(up => up.PinHash)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(up => up.User)
                .WithMany(u => u.Pins)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.DeviceRestriction)
                .WithMany()
                .HasForeignKey(up => up.DeviceRestrictionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index configurations
            builder.HasIndex(up => up.UserId);
            builder.HasIndex(up => up.DeviceRestrictionId);
        }
    }
}
