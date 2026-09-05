using SimplePubManager.Domain.Entities.Models;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a pub/organization that manages its staff, shifts, and operations.
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Unique identifier for the organization.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the organization/pub.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Subdomain for this organization (e.g., "tenant1" in "tenant1.myapp.com").
        /// </summary>
        public required string Subdomain { get; set; }

        /// <summary>
        /// Timestamp when the organization was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Collection of staff members in this organization.
        /// </summary>
        public ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Collection of areas/departments in this organization.
        /// </summary>
        public ICollection<Area> Areas { get; set; } = new List<Area>();

        /// <summary>
        /// Collection of shifts for this organization.
        /// </summary>
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

        /// <summary>
        /// Collection of tasks for this organization.
        /// </summary>
        public ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();

        /// <summary>
        /// Collection of holidays for this organization.
        /// </summary>
        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();

        /// <summary>
        /// Collection of bills for this organization.
        /// </summary>
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();

        /// <summary>
        /// Collection of payments for this organization.
        /// </summary>
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        /// <summary>
        /// Collection of devices registered to this organization.
        /// </summary>
        public ICollection<Device> Devices { get; set; } = new List<Device>();

        /// <summary>
        /// Collection of recurring task templates for this organization.
        /// </summary>
        public ICollection<RecurringTaskTemplate> RecurringTaskTemplates { get; set; } = new List<RecurringTaskTemplate>();
    }
}
