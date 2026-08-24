using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Organization entity.
    /// </summary>
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        /// <summary>
        /// Configures the Organization entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            // Key configuration
            builder.HasKey(o => o.Id);

            // Property configurations
            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255);

            // Index configurations
            builder.HasIndex(o => o.CreatedAt);

            // Relationship configurations
            builder.HasMany(o => o.Users)
                .WithOne(u => u.Organization)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Shifts)
                .WithOne(s => s.Organization)
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Areas)
                .WithOne(a => a.Organization)
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Holidays)
                .WithOne(h => h.Organization)
                .HasForeignKey(h => h.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Bills)
                .WithOne(b => b.Organization)
                .HasForeignKey(b => b.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Payments)
                .WithOne(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Devices)
                .WithOne(d => d.Organization)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.RecurringTaskTemplates)
                .WithOne(rtt => rtt.Organization)
                .HasForeignKey(rtt => rtt.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
