namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a device registered to the organization.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Unique identifier for the device.
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
