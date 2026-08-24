using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a staff member/user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization this user belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Full name of the user.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Hashed password for authentication.
        /// </summary>
        public required string PasswordHash { get; set; }

        /// <summary>
        /// Role of the user in the organization.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Current status of the user account.
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// Timestamp when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the organization this user belongs to.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Collection of shifts worked by this user.
        /// </summary>
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

        /// <summary>
        /// Collection of holidays requested by this user.
        /// </summary>
        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();

        /// <summary>
        /// Collection of tasks assigned to this user.
        /// </summary>
        public ICollection<Models.Task> AssignedTasks { get; set; } = new List<Models.Task>();

        /// <summary>
        /// Collection of PINs created for this user for quick-swap device login.
        /// </summary>
        public ICollection<UserPin> Pins { get; set; } = new List<UserPin>();

        /// <summary>
        /// Collection of active device sessions for this user.
        /// </summary>
        public ICollection<DeviceSession> DeviceSessions { get; set; } = new List<DeviceSession>();
    }
}
