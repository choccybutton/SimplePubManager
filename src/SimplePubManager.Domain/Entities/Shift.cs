using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a work shift in the organization.
    /// </summary>
    public class Shift
    {
        /// <summary>
        /// Unique identifier for the shift.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization this shift belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Foreign key to the staff member (User) who works this shift.
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// Type of shift (Planned or AdHoc).
        /// </summary>
        public ShiftType Type { get; set; }

        /// <summary>
        /// Start time of the shift.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of the shift (nullable for ongoing shifts).
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Current status of the shift.
        /// </summary>
        public ShiftStatus Status { get; set; }

        /// <summary>
        /// Foreign key to the user who created this shift.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Timestamp when the shift was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Navigation property to the staff member who works this shift.
        /// </summary>
        public User? Staff { get; set; }

        /// <summary>
        /// Navigation property to the user who created this shift.
        /// </summary>
        public User? CreatedByUser { get; set; }

        /// <summary>
        /// Collection of shift-to-area assignments.
        /// </summary>
        public ICollection<ShiftArea> Areas { get; set; } = new List<ShiftArea>();

        /// <summary>
        /// Collection of time log entries for this shift.
        /// </summary>
        public ICollection<ShiftLog> TimeLogs { get; set; } = new List<ShiftLog>();

        /// <summary>
        /// Payment record for this shift (one-to-one relationship).
        /// </summary>
        public ShiftPayment? Payment { get; set; }
    }
}
