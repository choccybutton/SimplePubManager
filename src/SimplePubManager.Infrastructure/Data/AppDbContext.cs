using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Entities.Models;
using Task = SimplePubManager.Domain.Entities.Models.Task;

namespace SimplePubManager.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for SimplePubManager application.
    /// Manages all database entities and their configurations.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">DbContext options configuration</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Core organization entities
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Area> Areas => Set<Area>();

        // Shift-related entities
        public DbSet<Shift> Shifts => Set<Shift>();
        public DbSet<ShiftArea> ShiftAreas => Set<ShiftArea>();
        public DbSet<ShiftLog> ShiftLogs => Set<ShiftLog>();
        public DbSet<ShiftPayment> ShiftPayments => Set<ShiftPayment>();

        // Task-related entities
        public DbSet<Task> Tasks => Set<Task>();
        public DbSet<RecurringTaskTemplate> RecurringTaskTemplates => Set<RecurringTaskTemplate>();

        // HR and administrative entities
        public DbSet<Holiday> Holidays => Set<Holiday>();
        public DbSet<Bill> Bills => Set<Bill>();
        public DbSet<Payment> Payments => Set<Payment>();

        // Device and session entities
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<UserPin> UserPins => Set<UserPin>();
        public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

        /// <summary>
        /// Configures the database model using Fluent API.
        /// Applies all entity configurations from the assembly.
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
