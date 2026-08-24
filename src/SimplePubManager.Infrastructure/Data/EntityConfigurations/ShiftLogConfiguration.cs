using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the ShiftLog entity.
    /// </summary>
    public class ShiftLogConfiguration : IEntityTypeConfiguration<ShiftLog>
    {
        /// <summary>
        /// Configures the ShiftLog entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<ShiftLog> builder)
        {
            // Key configuration
            builder.HasKey(sl => sl.Id);

            // Property configurations
            builder.Property(sl => sl.ShiftId)
                .IsRequired();

            builder.Property(sl => sl.ClockInTime)
                .IsRequired();

            builder.Property(sl => sl.Status)
                .IsRequired()
                .HasConversion<int>();

            // Foreign key configuration
            builder.HasOne(sl => sl.Shift)
                .WithMany(s => s.TimeLogs)
                .HasForeignKey(sl => sl.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations
            builder.HasIndex(sl => sl.ShiftId);
            builder.HasIndex(sl => sl.Status);
        }
    }
}
