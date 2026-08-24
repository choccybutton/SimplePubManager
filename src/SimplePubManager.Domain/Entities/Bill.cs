namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a bill or invoice for the organization.
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// Unique identifier for the bill.
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
