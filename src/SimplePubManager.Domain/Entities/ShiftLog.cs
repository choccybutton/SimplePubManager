using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a time tracking entry for a shift (clock in/out).
    /// </summary>
    public class ShiftLog
    {
        /// <summary>
        /// Unique identifier for the time log entry.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the shift this log entry belongs to.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Timestamp when the staff member clocked in.
        /// </summary>
        public DateTime ClockInTime { get; set; }

        /// <summary>
        /// Timestamp when the staff member clocked out (nullable for ongoing sessions).
        /// </summary>
        public DateTime? ClockOutTime { get; set; }

        /// <summary>
        /// Current status of the time log (ClockedIn or ClockedOut).
        /// </summary>
        public ShiftLogStatus Status { get; set; }

        /// <summary>
        /// Timestamp when this log entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the shift.
        /// </summary>
        public Shift? Shift { get; set; }
    }
}
