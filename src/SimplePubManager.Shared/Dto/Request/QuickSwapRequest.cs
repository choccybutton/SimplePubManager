namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to perform a quick-swap login on a shared device using PIN.
    /// </summary>
    public class QuickSwapRequest
    {
        /// <summary>
        /// The device ID (string identifier).
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// The user ID performing the quick-swap.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The user's PIN for quick-swap authentication.
        /// </summary>
        public string Pin { get; set; } = string.Empty;
    }
}
