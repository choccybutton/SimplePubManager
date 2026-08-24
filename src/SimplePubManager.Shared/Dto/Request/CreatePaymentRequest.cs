namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to record a payment for staff.
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>
        /// The ID of the staff member receiving the payment (optional).
        /// </summary>
        public Guid? StaffId { get; set; }

        /// <summary>
        /// The amount of the payment.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The type of payment.
        /// </summary>
        public string Type { get; set; } = "ShiftPayment";

        /// <summary>
        /// The ID of the related shift (optional).
        /// </summary>
        public Guid? RelatedShiftId { get; set; }
    }
}
