namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a shared tablet/kiosk device for quick-swap staff login.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Unique identifier for the device.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization this device belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Human-chosen identifier for the device (e.g., "Bar-Tablet-1").
        /// </summary>
        public required string DeviceId { get; set; }

        /// <summary>
        /// Bcrypt hashed device key for authentication.
        /// </summary>
        public required string DeviceKeyHash { get; set; }

        /// <summary>
        /// Display name for the device.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Whether the device is currently enabled for use.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Physical location of the device (optional).
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Timestamp of the last location update.
        /// </summary>
        public DateTime? LastLocationUpdate { get; set; }

        /// <summary>
        /// Timestamp when the device was registered.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the organization this device belongs to.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Collection of active sessions on this device.
        /// </summary>
        public ICollection<DeviceSession> Sessions { get; set; } = new List<DeviceSession>();
    }
}
