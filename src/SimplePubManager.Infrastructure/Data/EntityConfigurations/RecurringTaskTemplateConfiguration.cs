using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the RecurringTaskTemplate entity.
    /// </summary>
    public class RecurringTaskTemplateConfiguration : IEntityTypeConfiguration<RecurringTaskTemplate>
    {
        /// <summary>
        /// Configures the RecurringTaskTemplate entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<RecurringTaskTemplate> builder)
        {
            // Key configuration
            builder.HasKey(rtt => rtt.Id);

            // Property configurations
            builder.Property(rtt => rtt.OrganizationId)
                .IsRequired();

            builder.Property(rtt => rtt.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(rtt => rtt.Description)
                .HasMaxLength(2000);

            builder.Property(rtt => rtt.RecurrencePattern)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(rtt => rtt.NextOccurrenceDate)
                .IsRequired();

            builder.Property(rtt => rtt.Active)
                .IsRequired();

            // Foreign key configurations
            builder.HasOne(rtt => rtt.Organization)
                .WithMany(o => o.RecurringTaskTemplates)
                .HasForeignKey(rtt => rtt.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rtt => rtt.AssignedUser)
                .WithMany()
                .HasForeignKey(rtt => rtt.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(rtt => rtt.AssignedArea)
                .WithMany()
                .HasForeignKey(rtt => rtt.AssignedToAreaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index configurations
            builder.HasIndex(rtt => rtt.OrganizationId);
            builder.HasIndex(rtt => rtt.Active);
            builder.HasIndex(rtt => rtt.NextOccurrenceDate);
        }
    }
}
