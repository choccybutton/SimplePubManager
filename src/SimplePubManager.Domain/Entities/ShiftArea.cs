namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents the assignment of an area to a shift.
    /// </summary>
    public class ShiftArea
    {
        /// <summary>
        /// Unique identifier for the shift-area assignment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the shift.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Foreign key to the area.
        /// </summary>
        public Guid AreaId { get; set; }

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
