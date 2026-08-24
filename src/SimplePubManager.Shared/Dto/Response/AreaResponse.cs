namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing work area information.
    /// </summary>
    public class AreaResponse
    {
        /// <summary>
        /// The area ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The area name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The area description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// When the area was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
