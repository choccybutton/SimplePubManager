using SimplePubManager.Domain.Entities.Models;

namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a department, location, or area within an organization.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Unique identifier for the area.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization this area belongs to.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Name of the area (e.g., "Bar", "Kitchen", "Front of House").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Description of the area's purpose and responsibilities.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Timestamp when the area was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the organization this area belongs to.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Collection of shift-to-area assignments for this area.
        /// </summary>
        public ICollection<ShiftArea> ShiftAreas { get; set; } = new List<ShiftArea>();

        /// <summary>
        /// Collection of tasks assigned to this area.
        /// </summary>
        public ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();
    }
}
