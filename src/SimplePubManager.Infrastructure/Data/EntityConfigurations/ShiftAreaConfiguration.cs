using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the ShiftArea entity (junction table).
    /// </summary>
    public class ShiftAreaConfiguration : IEntityTypeConfiguration<ShiftArea>
    {
        /// <summary>
        /// Configures the ShiftArea entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<ShiftArea> builder)
        {
            // Composite key configuration
            builder.HasKey(sa => new { sa.ShiftId, sa.AreaId });

            // Foreign key configurations
            builder.HasOne(sa => sa.Shift)
                .WithMany(s => s.Areas)
                .HasForeignKey(sa => sa.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sa => sa.Area)
                .WithMany(a => a.ShiftAreas)
                .HasForeignKey(sa => sa.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for AreaId for queries filtering by area
            builder.HasIndex(sa => sa.AreaId);
        }
    }
}
