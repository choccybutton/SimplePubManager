using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for user-related data operations.
    /// </summary>
    public class UserRepository : BaseRepository<User>
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a user by email within a specific organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="email">The email address to search for</param>
        /// <returns>The user if found; otherwise null</returns>
        public async Task<User?> GetUserByEmailAsync(Guid orgId, string email)
        {
            return await Query()
                .FirstOrDefaultAsync(u => u.OrganizationId == orgId && u.Email == email);
        }

        /// <summary>
        /// Retrieves all users in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of users in the organization</returns>
        public async Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid orgId)
        {
            return await Query()
                .Where(u => u.OrganizationId == orgId)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all users with a specific role in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="role">The user role to filter by</param>
        /// <returns>An enumerable collection of users with the specified role</returns>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid orgId, UserRole role)
        {
            return await Query()
                .Where(u => u.OrganizationId == orgId && u.Role == role)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active users in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of active users</returns>
        public async Task<IEnumerable<User>> GetActiveUsersAsync(Guid orgId)
        {
            return await Query()
                .Where(u => u.OrganizationId == orgId && u.Status == UserStatus.Active)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
    }
}
