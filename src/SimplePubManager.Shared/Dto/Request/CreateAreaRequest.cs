namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to create a new work area.
    /// </summary>
    public class CreateAreaRequest
    {
        /// <summary>
        /// The name of the area.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the area.
        /// </summary>
        public string? Description { get; set; }
    }
}
