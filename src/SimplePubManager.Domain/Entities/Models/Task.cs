using SimplePubManager.Domain.Enums;

namespace SimplePubManager.Domain.Entities.Models
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
        /// Title of the task.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Optional description of the task.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Foreign key to the user assigned to this task (optional).
        /// </summary>
        public Guid? AssignedToUserId { get; set; }

        /// <summary>
        /// Foreign key to the area assigned to this task (optional).
        /// </summary>
        public Guid? AssignedToAreaId { get; set; }

        /// <summary>
        /// Due date for the task.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Current status of the task.
        /// </summary>
        public Enums.TaskStatus Status { get; set; }

        /// <summary>
        /// Foreign key to the user who completed this task (optional).
        /// </summary>
        public Guid? CompletedBy { get; set; }

        /// <summary>
        /// Timestamp when the task was completed (optional).
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Optional notes on the completion of the task.
        /// </summary>
        public string? CompletionNotes { get; set; }

        /// <summary>
        /// Timestamp when the task was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // Navigation properties

        /// <summary>
        /// Navigation property to the organization.
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Navigation property to the assigned user.
        /// </summary>
        public User? AssignedUser { get; set; }

        /// <summary>
        /// Navigation property to the assigned area.
        /// </summary>
        public Area? AssignedArea { get; set; }

        /// <summary>
        /// Navigation property to the user who completed this task.
        /// </summary>
        public User? CompletedByUser { get; set; }
    }
}
