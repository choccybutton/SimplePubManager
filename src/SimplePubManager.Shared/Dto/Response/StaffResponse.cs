namespace SimplePubManager.Shared.Dto.Response
{
    /// <summary>
    /// Response containing staff member information.
    /// </summary>
    public class StaffResponse
    {
        /// <summary>
        /// The staff member ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The staff member's full name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The staff member's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The staff member's role in the organization.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// The staff member's current status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// When the staff member was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
