namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to authenticate a device using device ID and key.
    /// </summary>
    public class DeviceLoginRequest
    {
        /// <summary>
        /// The device identifier (string).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// The device key for authentication.
        /// </summary>
        public string DeviceKey { get; set; } = string.Empty;
    }
}
