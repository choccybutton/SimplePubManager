namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing authentication token and user information.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// The JWT token for authenticated requests.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// The authenticated user's ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The authenticated user's name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// The authenticated user's email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's role in the organization.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// The organization ID.
        /// </summary>
        public Guid OrganizationId { get; set; }
    }
}
