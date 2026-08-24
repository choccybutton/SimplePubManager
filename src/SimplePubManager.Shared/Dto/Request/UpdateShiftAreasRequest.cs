namespace SimplePubManager.Shared.Dto.Request
{
    /// <summary>
    /// Request to update the areas assigned to a shift.
    /// </summary>
    public class UpdateShiftAreasRequest
    {
        /// <summary>
        /// List of area IDs to assign to the shift.
        /// </summary>
        public List<Guid> AreaIds { get; set; } = new();
    }
}
