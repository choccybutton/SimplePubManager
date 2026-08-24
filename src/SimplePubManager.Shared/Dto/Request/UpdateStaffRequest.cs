namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update staff member information.
    /// </summary>
    public class UpdateStaffRequest
    {
        /// <summary>
        /// The staff member's name (optional).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The staff member's email (optional).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The staff member's role (optional).
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// The staff member's status (optional).
        /// </summary>
        public string? Status { get; set; }
    }
}
