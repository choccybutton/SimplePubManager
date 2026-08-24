namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents the assignment of an area to a shift.
    /// Junction table: many shifts can cover many areas.
    /// </summary>
    public class ShiftArea
    {
        /// <summary>
        /// Foreign key to the shift (part of composite key).
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Foreign key to the area (part of composite key).
        /// </summary>
        public Guid AreaId { get; set; }

        /// <summary>
        /// Timestamp when the area was assigned to this shift.
        /// </summary>
        public DateTime AssignedAt { get; set; }

        /// <summary>
        /// Navigation property to the shift.
        /// </summary>
        public Shift? Shift { get; set; }

        /// <summary>
        /// Navigation property to the area.
        /// </summary>
        public Area? Area { get; set; }
    }
}
