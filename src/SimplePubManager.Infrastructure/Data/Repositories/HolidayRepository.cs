using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for holiday-related data operations.
    /// </summary>
    public class HolidayRepository : BaseRepository<Holiday>
    {
        /// <summary>
        /// Initializes a new instance of the HolidayRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public HolidayRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all holidays for a specific staff member.
        /// </summary>
        /// <param name="staffId">The staff member ID</param>
        /// <returns>An enumerable collection of holidays for the staff member</returns>
        public async Task<IEnumerable<Holiday>> GetHolidaysByStaffAsync(Guid staffId)
        {
            return await Query()
                .Where(h => h.StaffId == staffId)
                .OrderByDescending(h => h.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all pending holiday requests in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of pending holiday requests</returns>
        public async Task<IEnumerable<Holiday>> GetPendingHolidaysAsync(Guid orgId)
        {
            return await Query()
                .Where(h => h.OrganizationId == orgId && h.Status == HolidayStatus.Pending)
                .OrderByDescending(h => h.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all holidays with a specific status in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">The holiday status to filter by</param>
        /// <returns>An enumerable collection of holidays with the specified status</returns>
        public async Task<IEnumerable<Holiday>> GetHolidaysByStatusAsync(Guid orgId, HolidayStatus status)
        {
            return await Query()
                .Where(h => h.OrganizationId == orgId && h.Status == status)
                .OrderByDescending(h => h.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a staff member has conflicting holiday requests during a specified date range.
        /// </summary>
        /// <param name="staffId">The staff member ID</param>
        /// <param name="startDate">The start date of the period to check</param>
        /// <param name="endDate">The end date of the period to check</param>
        /// <returns>True if a conflict exists; otherwise false</returns>
        public async Task<bool> CheckHolidayConflictAsync(Guid staffId, DateTime startDate, DateTime endDate)
        {
            return await Query()
                .Where(h => h.StaffId == staffId &&
                       h.Status != HolidayStatus.Rejected &&
                       h.StartDate < endDate &&
                       h.EndDate > startDate)
                .AnyAsync();
        }
    }
}
