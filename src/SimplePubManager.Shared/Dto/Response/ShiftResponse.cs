namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing shift information.
    /// </summary>
    public class ShiftResponse
    {
        /// <summary>
        /// The shift ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The staff member assigned to the shift.
        /// </summary>
        public Guid StaffId { get; set; }

        /// <summary>
        /// The type of shift (Planned or AdHoc).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The shift start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The shift end time (if completed).
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// The current status of the shift.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The IDs of areas assigned to this shift.
        /// </summary>
        public List<Guid> AreaIds { get; set; } = new();

        /// <summary>
        /// When the shift was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
