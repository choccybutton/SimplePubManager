namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to create a new task.
    /// </summary>
    public class CreateTaskRequest
    {
        /// <summary>
        /// The title of the task.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The description of the task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the user assigned to the task (optional).
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// The ID of the area where the task should be performed (optional).
        /// </summary>
        public Guid? AreaId { get; set; }
    }
}
