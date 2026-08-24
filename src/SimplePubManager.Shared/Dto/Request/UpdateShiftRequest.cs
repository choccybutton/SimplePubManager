namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update an existing shift.
    /// </summary>
    public class UpdateShiftRequest
    {
        /// <summary>
        /// The end time of the shift (optional).
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// The new status of the shift (optional).
        /// </summary>
        public string? Status { get; set; }
    }
}
