using Microsoft.EntityFrameworkCore;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Generic base repository providing common CRUD and query operations for all entity types.
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages</typeparam>
    public class BaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the BaseRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity if found; otherwise null</returns>
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <returns>An enumerable collection of all entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update</param>
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if an entity with the specified ID exists.
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>True if the entity exists; otherwise false</returns>
        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        /// <summary>
        /// Returns an IQueryable for building custom queries against the entity set.
        /// </summary>
        /// <returns>An IQueryable for the entity type</returns>
        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
    }
}
