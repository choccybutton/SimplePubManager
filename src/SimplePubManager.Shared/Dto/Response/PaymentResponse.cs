namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing payment information.
    /// </summary>
    public class PaymentResponse
    {
        /// <summary>
        /// The payment ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the bill being paid (if any).
        /// </summary>
        public Guid? BillId { get; set; }

        /// <summary>
        /// The payment amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The payment method.
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// The status of the payment.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The reference number for the payment.
        /// </summary>
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// When the payment was recorded.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
