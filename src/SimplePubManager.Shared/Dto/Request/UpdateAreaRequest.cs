namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update a work area.
    /// </summary>
    public class UpdateAreaRequest
    {
        /// <summary>
        /// The name of the area (optional).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optional description of the area.
        /// </summary>
        public string? Description { get; set; }
    }
}
