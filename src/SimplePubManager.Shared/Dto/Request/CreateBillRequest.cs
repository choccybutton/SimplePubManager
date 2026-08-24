namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to create a new bill.
    /// </summary>
    public class CreateBillRequest
    {
        /// <summary>
        /// The description of the bill.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The amount of the bill.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The due date for the bill.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Optional vendor or supplier name.
        /// </summary>
        public string? Vendor { get; set; }
    }
}
