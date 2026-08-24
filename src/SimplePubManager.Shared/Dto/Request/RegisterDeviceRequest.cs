namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to register a new shared device.
    /// </summary>
    public class RegisterDeviceRequest
    {
        /// <summary>
        /// The device identifier (string).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// The device key for authentication.
        /// </summary>
        public string DeviceKey { get; set; } = string.Empty;

        /// <summary>
        /// The display name for the device.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
