namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update a device's settings.
    /// </summary>
    public class UpdateDeviceRequest
    {
        /// <summary>
        /// The display name for the device (optional).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Whether the device is enabled (optional).
        /// </summary>
        public bool? Enabled { get; set; }
    }
}
