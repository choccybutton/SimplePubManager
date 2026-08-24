using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for bill-related data operations.
    /// </summary>
    public class BillRepository : BaseRepository<Bill>
    {
        /// <summary>
        /// Initializes a new instance of the BillRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public BillRepository(AppDbContext context) : base(context)
        {
        }
    }
}
