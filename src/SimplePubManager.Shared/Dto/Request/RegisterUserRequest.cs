namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to register a new user.
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// The user's full name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The user's role in the organization.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
