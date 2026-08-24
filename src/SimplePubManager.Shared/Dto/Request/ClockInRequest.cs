namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to clock in for a shift.
    /// </summary>
    public class ClockInRequest
    {
        /// <summary>
        /// Optional timestamp for the clock in (defaults to now).
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
