namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents an active authentication session on a shared device.
    /// </summary>
    public class DeviceSession
    {
        /// <summary>
        /// Unique identifier for the session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the device this session is on.
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// Foreign key to the user logged in for this session.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Session token (JWT) for this session.
        /// </summary>
        public required string SessionToken { get; set; }

        /// <summary>
        /// Timestamp when the session was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the session will expire.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Timestamp of the last activity in this session.
        /// </summary>
        public DateTime LastActivityAt { get; set; }

        /// <summary>
        /// Navigation property to the device this session is on.
        /// </summary>
        public Device? Device { get; set; }

        /// <summary>
        /// Navigation property to the user logged in for this session.
        /// </summary>
        public User? User { get; set; }
    }
}
