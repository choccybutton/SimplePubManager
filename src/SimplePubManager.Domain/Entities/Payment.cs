using SimplePubManager.Domain.Enums;

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
        /// Foreign key to the staff member receiving the payment (optional).
        /// </summary>
        public Guid? StaffId { get; set; }

        /// <summary>
        /// Amount of the payment.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Type of payment.
        /// </summary>
        public PaymentType Type { get; set; }

        /// <summary>
        /// Foreign key to the related shift (optional).
        /// </summary>
        public Guid? RelatedShiftId { get; set; }

        /// <summary>
        /// Date of the payment.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Timestamp when the payment was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // Navigation properties

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Navigation property to the staff member.
        /// </summary>
        public User? Staff { get; set; }

        /// <summary>
        /// Navigation property to the related shift.
        /// </summary>
        public Shift? RelatedShift { get; set; }
    }
}
