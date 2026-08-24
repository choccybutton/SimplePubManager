using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for recurring task template-related data operations.
    /// </summary>
    public class RecurringTaskTemplateRepository : BaseRepository<RecurringTaskTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the RecurringTaskTemplateRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public RecurringTaskTemplateRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all active recurring task templates in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of active templates</returns>
        public async Task<IEnumerable<RecurringTaskTemplate>> GetActiveTemplatesAsync(Guid orgId)
        {
            return await Query()
                .Where(t => t.OrganizationId == orgId && t.Active)
                .OrderBy(t => t.NextOccurrenceDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all recurring task templates in an organization that are due to generate tasks today or earlier.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of templates due for task generation</returns>
        public async Task<IEnumerable<RecurringTaskTemplate>> GetDueTemplatesAsync(Guid orgId)
        {
            var today = DateTime.UtcNow.Date;
            return await Query()
                .Where(t => t.OrganizationId == orgId &&
                           t.Active &&
                           t.NextOccurrenceDate.Date <= today)
                .OrderBy(t => t.NextOccurrenceDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all recurring task templates assigned to a specific user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>An enumerable collection of templates assigned to the user</returns>
        public async Task<IEnumerable<RecurringTaskTemplate>> GetTemplatesByAssigneeAsync(Guid userId)
        {
            return await Query()
                .Where(t => t.AssignedToUserId == userId && t.Active)
                .OrderBy(t => t.NextOccurrenceDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all recurring task templates assigned to a specific area.
        /// </summary>
        /// <param name="areaId">The area ID</param>
        /// <returns>An enumerable collection of templates assigned to the area</returns>
        public async Task<IEnumerable<RecurringTaskTemplate>> GetTemplatesByAreaAsync(Guid areaId)
        {
            return await Query()
                .Where(t => t.AssignedToAreaId == areaId && t.Active)
                .OrderBy(t => t.NextOccurrenceDate)
                .ToListAsync();
        }
    }
}
