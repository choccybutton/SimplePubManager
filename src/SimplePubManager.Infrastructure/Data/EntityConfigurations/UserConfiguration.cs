using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePubManager.Domain.Entities;

namespace SimplePubManager.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Fluent API configuration for the User entity.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the User entity.
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Key configuration
            builder.HasKey(u => u.Id);

            // Foreign key configuration
            builder.HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property configurations
            builder.Property(u => u.OrganizationId)
                .IsRequired();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion<int>();

            // Unique constraint for (OrganizationId, Email)
            builder.HasIndex(u => new { u.OrganizationId, u.Email })
                .IsUnique();

            // Individual indices for frequently queried columns
            builder.HasIndex(u => u.OrganizationId);
            builder.HasIndex(u => u.Status);

            // Relationship configurations
            builder.HasMany(u => u.Shifts)
                .WithOne(s => s.Staff)
                .HasForeignKey(s => s.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Holidays)
                .WithOne(h => h.Staff)
                .HasForeignKey(h => h.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.AssignedTasks)
                .WithOne(t => t.AssignedUser)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.Pins)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.DeviceSessions)
                .WithOne(ds => ds.User)
                .HasForeignKey(ds => ds.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Self-referential relationships for CreatedBy on Shifts
            builder.HasMany<Shift>()
                .WithOne(s => s.CreatedByUser)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            // Self-referential relationships for CompletedBy on Tasks
            builder.HasMany<Domain.Entities.Models.Task>()
                .WithOne(t => t.CompletedByUser)
                .HasForeignKey(t => t.CompletedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Self-referential relationships for ApprovedBy on Holidays
            builder.HasMany<Holiday>()
                .WithOne(h => h.ApprovedByUser)
                .HasForeignKey(h => h.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
