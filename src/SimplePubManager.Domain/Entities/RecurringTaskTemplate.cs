namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a template for recurring tasks in the organization.
    /// </summary>
    public class RecurringTaskTemplate
    {
        /// <summary>
        /// Unique identifier for the recurring task template.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }
    }
}
