using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for shift-related data operations.
    /// </summary>
    public class ShiftRepository : BaseRepository<Shift>
    {
        /// <summary>
        /// Initializes a new instance of the ShiftRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public ShiftRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves shifts for an organization with a specific status, with pagination support.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">The shift status to filter by</param>
        /// <param name="page">The page number (1-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>An enumerable collection of shifts matching the criteria</returns>
        public async Task<IEnumerable<Shift>> GetShiftsByOrganizationAndStatusAsync(
            Guid orgId, ShiftStatus status, int page, int pageSize)
        {
            return await Query()
                .Where(s => s.OrganizationId == orgId && s.Status == status)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all shifts for a specific staff member.
        /// </summary>
        /// <param name="staffId">The staff member ID</param>
        /// <returns>An enumerable collection of shifts for the staff member</returns>
        public async Task<IEnumerable<Shift>> GetShiftsByStaffAsync(Guid staffId)
        {
            return await Query()
                .Where(s => s.StaffId == staffId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all pending shift approvals for an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of pending shifts</returns>
        public async Task<IEnumerable<Shift>> GetPendingApprovalsAsync(Guid orgId)
        {
            return await Query()
                .Where(s => s.OrganizationId == orgId && s.Status == ShiftStatus.PendingApproval)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a shift with its associated areas.
        /// </summary>
        /// <param name="shiftId">The shift ID</param>
        /// <returns>The shift with areas included, or null if not found</returns>
        public async Task<Shift?> GetShiftWithAreasAsync(Guid shiftId)
        {
            return await Query()
                .Include(s => s.Areas)
                .FirstOrDefaultAsync(s => s.Id == shiftId);
        }
    }
}
