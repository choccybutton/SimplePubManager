using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a bill or invoice for the organization.
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// Unique identifier for the bill.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Description of the bill.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Amount of the bill.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Due date for the bill.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date when the bill was paid (optional).
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// Current status of the bill.
        /// </summary>
        public BillStatus Status { get; set; }

        /// <summary>
        /// Timestamp when the bill was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // Navigation properties

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }
    }
}
