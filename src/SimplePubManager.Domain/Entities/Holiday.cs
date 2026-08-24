namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a holiday or time-off period in the organization.
    /// </summary>
    public class Holiday
    {
        /// <summary>
        /// Unique identifier for the holiday.
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
