namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to create a new shift.
    /// </summary>
    public class CreateShiftRequest
    {
        /// <summary>
        /// The ID of the staff member assigned to the shift.
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// The type of shift (Planned or AdHoc).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The start time of the shift.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The end time of the shift (optional for ongoing shifts).
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Optional area IDs to assign to this shift.
        /// </summary>
        public List<Guid>? AreaIds { get; set; }
    }
}
