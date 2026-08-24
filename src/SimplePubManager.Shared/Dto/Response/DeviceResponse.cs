namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing device information.
    /// </summary>
    public class DeviceResponse
    {
        /// <summary>
        /// The device ID (Guid).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The device identifier (string).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// The display name for the device.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Whether the device is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// When the device was registered.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
