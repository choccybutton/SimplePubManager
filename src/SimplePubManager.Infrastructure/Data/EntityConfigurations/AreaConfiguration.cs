using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Area entity.
    /// </summary>
    public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        /// <summary>
        /// Configures the Area entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            // Key configuration
            builder.HasKey(a => a.Id);

            // Property configurations
            builder.Property(a => a.OrganizationId)
                .IsRequired();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Description)
                .HasMaxLength(500);

            // Unique constraint for (OrganizationId, Name)
            builder.HasIndex(a => new { a.OrganizationId, a.Name })
                .IsUnique();

            // Individual indices
            builder.HasIndex(a => a.OrganizationId);

            // Foreign key configuration
            builder.HasOne(a => a.Organization)
                .WithMany(o => o.Areas)
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship configurations
            builder.HasMany(a => a.ShiftAreas)
                .WithOne(sa => sa.Area)
                .HasForeignKey(sa => sa.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
