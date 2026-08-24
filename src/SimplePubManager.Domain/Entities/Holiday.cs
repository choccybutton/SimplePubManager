using SimplePubManager.Domain.Enums;

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
        /// Foreign key to the staff member taking the holiday.
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// Foreign key to the organization.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Start date of the holiday.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the holiday.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Type of holiday (e.g., "paid" or "unpaid").
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Current status of the holiday request.
        /// </summary>
        public HolidayStatus Status { get; set; }

        /// <summary>
        /// Timestamp when the holiday was requested.
        /// </summary>
        public DateTime RequestedAt { get; set; }

        /// <summary>
        /// Foreign key to the user who approved this holiday (optional).
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// Timestamp when the holiday was approved (optional).
        /// </summary>
        public DateTime? ApprovedAt { get; set; }

        // Navigation properties

        /// <summary>
        /// Navigation property to the staff member taking the holiday.
        /// </summary>
        public User? Staff { get; set; }

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Navigation property to the user who approved this holiday.
        /// </summary>
        public User? ApprovedByUser { get; set; }
    }
}
