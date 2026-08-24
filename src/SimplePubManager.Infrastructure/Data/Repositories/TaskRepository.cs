using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities.Models;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;
using Task = SimplePubManager.Domain.Entities.Models.Task;
using DomainTaskStatus = SimplePubManager.Domain.Enums.TaskStatus;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for task-related data operations.
    /// </summary>
    public class TaskRepository : BaseRepository<Task>
    {
        /// <summary>
        /// Initializes a new instance of the TaskRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public TaskRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all tasks with a specific status in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">The task status to filter by</param>
        /// <returns>An enumerable collection of tasks with the specified status</returns>
        public async Task<IEnumerable<Task>> GetTasksByStatusAsync(Guid orgId, DomainTaskStatus status)
        {
            return await Query()
                .Where(t => t.OrganizationId == orgId && t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all tasks assigned to a specific user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>An enumerable collection of tasks assigned to the user</returns>
        public async Task<IEnumerable<Task>> GetTasksByAssigneeAsync(Guid userId)
        {
            return await Query()
                .Where(t => t.AssignedToUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all tasks associated with a specific area.
        /// </summary>
        /// <param name="areaId">The area ID</param>
        /// <returns>An enumerable collection of tasks for the area</returns>
        public async Task<IEnumerable<Task>> GetTasksByAreaAsync(Guid areaId)
        {
            return await Query()
                .Where(t => t.AssignedToAreaId == areaId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all overdue tasks in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of overdue tasks</returns>
        public async Task<IEnumerable<Task>> GetOverdueTasksAsync(Guid orgId)
        {
            return await Query()
                .Where(t => t.OrganizationId == orgId &&
                       t.DueDate < DateTime.UtcNow &&
                       t.Status != DomainTaskStatus.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
    }
}
