namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update an existing task.
    /// </summary>
    public class UpdateTaskRequest
    {
        /// <summary>
        /// The title of the task (optional).
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The description of the task (optional).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The ID of the user assigned to the task (optional).
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// The status of the task (optional).
        /// </summary>
        public string? Status { get; set; }
    }
}
