using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Entities.Models;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the Task entity.
    /// </summary>
    public class TaskConfiguration : IEntityTypeConfiguration<SimplePubManager.Domain.Entities.Models.Task>
    {
        /// <summary>
        /// Configures the Task entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<SimplePubManager.Domain.Entities.Models.Task> builder)
        {
            // Key configuration
            builder.HasKey(t => t.Id);

            // Property configurations
            builder.Property(t => t.OrganizationId)
                .IsRequired();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.DueDate)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.CompletionNotes)
                .HasMaxLength(2000);

            // Foreign key configurations
            builder.HasOne(t => t.Organization)
                .WithMany("Tasks")
                .HasForeignKey(t => t.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.AssignedUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.AssignedArea)
                .WithMany("Tasks")
                .HasForeignKey(t => t.AssignedToAreaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.CompletedByUser)
                .WithMany()
                .HasForeignKey(t => t.CompletedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Index configurations
            builder.HasIndex(t => t.OrganizationId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.DueDate);
            builder.HasIndex(t => t.AssignedToUserId);
        }
    }
}
