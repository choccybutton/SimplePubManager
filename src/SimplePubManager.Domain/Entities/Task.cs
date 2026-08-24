namespace SimplePubManager.Domain.Entities
{
    /// <summary>
    /// Represents a task or work item in the organization.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Unique identifier for the task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key to the organization.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Foreign key to the area (if task is area-specific).
        /// </summary>
        public Guid? AreaId { get; set; }

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Navigation property to the area.
        /// </summary>
        public Area? Area { get; set; }
    }
}
