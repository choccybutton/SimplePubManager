namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing task information.
    /// </summary>
    public class TaskResponse
    {
        /// <summary>
        /// The task ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The task title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The task description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the user assigned to the task (if any).
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// The status of the task.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the area where the task is performed (if any).
        /// </summary>
        public Guid? AreaId { get; set; }

        /// <summary>
        /// When the task was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
