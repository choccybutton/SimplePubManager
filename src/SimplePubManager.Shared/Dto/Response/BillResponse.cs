namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing bill information.
    /// </summary>
    public class BillResponse
    {
        /// <summary>
        /// The bill ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The description of the bill.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The bill amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The due date for the bill.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The status of the bill.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The vendor or supplier name.
        /// </summary>
        public string? Vendor { get; set; }

        /// <summary>
        /// When the bill was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
