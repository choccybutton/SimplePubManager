using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for area-related data operations.
    /// </summary>
    public class AreaRepository : BaseRepository<Area>
    {
        /// <summary>
        /// Initializes a new instance of the AreaRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public AreaRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all areas in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of areas in the organization</returns>
        public async Task<IEnumerable<Area>> GetAreasByOrganizationAsync(Guid orgId)
        {
            return await Query()
                .Where(a => a.OrganizationId == orgId)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves an area by name within an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="name">The area name to search for</param>
        /// <returns>The area if found; otherwise null</returns>
        public async Task<Area?> GetAreaByNameAsync(Guid orgId, string name)
        {
            return await Query()
                .FirstOrDefaultAsync(a => a.OrganizationId == orgId && a.Name == name);
        }
    }
}
