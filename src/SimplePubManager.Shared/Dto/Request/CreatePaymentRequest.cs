namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to record a payment.
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>
        /// The ID of the bill being paid (optional).
        /// </summary>
        public Guid? BillId { get; set; }

        /// <summary>
        /// The amount of the payment.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The payment method.
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// Optional reference number for the payment.
        /// </summary>
        public string? ReferenceNumber { get; set; }
    }
}
