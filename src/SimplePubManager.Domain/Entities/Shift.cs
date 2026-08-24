namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a work shift in the organization.
    /// </summary>
    public class Shift
    {
        /// <summary>
        /// Unique identifier for the shift.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization this shift belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Collection of shift-to-area assignments.
        /// </summary>
        public ICollection<ShiftArea> ShiftAreas { get; set; } = new List<ShiftArea>();
    }
}
