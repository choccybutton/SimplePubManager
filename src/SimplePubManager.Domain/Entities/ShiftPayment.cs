using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents payment calculation for a completed shift.
    /// </summary>
    public class ShiftPayment
    {
        /// <summary>
        /// Unique identifier for the payment record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the shift this payment is for.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Hourly rate for this shift.
        /// </summary>
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Total hours worked in this shift.
        /// </summary>
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Total payment amount (HourlyRate * HoursWorked).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Current status of the payment.
        /// </summary>
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Timestamp when the payment record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the shift.
        /// </summary>
        public Shift? Shift { get; set; }
    }
}
