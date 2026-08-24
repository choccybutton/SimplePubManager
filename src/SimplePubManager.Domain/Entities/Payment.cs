namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a payment in the organization.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Unique identifier for the payment.
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
