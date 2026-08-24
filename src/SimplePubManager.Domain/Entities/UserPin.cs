namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a PIN for quick-swap authentication on shared devices.
    /// </summary>
    public class UserPin
    {
        /// <summary>
        /// Unique identifier for the PIN record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the user who owns this PIN.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Bcrypt hashed PIN value for authentication.
        /// </summary>
        public required string PinHash { get; set; }

        /// <summary>
        /// Optional foreign key to restrict this PIN to a specific device.
        /// </summary>
        public Guid? DeviceRestrictionId { get; set; }

        /// <summary>
        /// Timestamp when the PIN was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the user who owns this PIN.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Navigation property to the device this PIN is restricted to (if any).
        /// </summary>
        public Device? DeviceRestriction { get; set; }
    }
}
