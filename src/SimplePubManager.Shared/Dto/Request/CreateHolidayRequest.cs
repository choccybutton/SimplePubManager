namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to create a holiday request.
    /// </summary>
    public class CreateHolidayRequest
    {
        /// <summary>
        /// The start date of the holiday.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the holiday.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Optional reason or notes for the holiday.
        /// </summary>
        public string? Reason { get; set; }
    }
}
