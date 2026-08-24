namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing holiday request information.
    /// </summary>
    public class HolidayResponse
    {
        /// <summary>
        /// The holiday request ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the staff member requesting the holiday.
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// The start date of the holiday.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the holiday.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The status of the holiday request.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The reason for the holiday request.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// When the holiday request was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
