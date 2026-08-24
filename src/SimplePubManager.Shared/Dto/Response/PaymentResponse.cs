namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing payment information for staff.
    /// </summary>
    public class PaymentResponse
    {
        /// <summary>
        /// The payment ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the staff member receiving the payment.
        /// </summary>
        public Guid? StaffId { get; set; }

        /// <summary>
        /// The payment amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The type of payment.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the related shift.
        /// </summary>
        public Guid? RelatedShiftId { get; set; }

        /// <summary>
        /// The date of the payment.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// When the payment was recorded.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
