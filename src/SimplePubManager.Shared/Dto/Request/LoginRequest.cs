namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to authenticate a user with email and password.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The organization ID for context.
        /// </summary>
        public Guid OrganizationId { get; set; }
    }
}
