using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for Organization entity operations, including tenant resolution by subdomain.
    /// </summary>
    public class OrganizationRepository : BaseRepository<Organization>
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public OrganizationRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Finds an organization by its subdomain.
        /// </summary>
        /// <param name="subdomain">The subdomain to search for (e.g., "tenant1" from "tenant1.myapp.com")</param>
        /// <returns>The organization if found; otherwise null</returns>
        public async Task<Organization?> FindBySubdomainAsync(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
            {
                return null;
            }

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Subdomain.ToLower() == subdomain.ToLower());
        }
    }
}
