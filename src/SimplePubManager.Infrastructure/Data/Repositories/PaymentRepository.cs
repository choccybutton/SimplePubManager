using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for payment-related data operations.
    /// </summary>
    public class PaymentRepository : BaseRepository<Payment>
    {
        /// <summary>
        /// Initializes a new instance of the PaymentRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all payments for a specific staff member.
        /// </summary>
        /// <param name="staffId">The staff member ID</param>
        /// <returns>An enumerable collection of payments for the staff member</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByStaffAsync(Guid staffId)
        {
            return await Query()
                .Where(p => p.StaffId == staffId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all payments for an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of payments in the organization</returns>
        public async Task<IEnumerable<Payment>> GetPaymentsByOrganizationAsync(Guid orgId)
        {
            return await Query()
                .Where(p => p.OrganizationId == orgId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Calculates the total amount owed to a staff member.
        /// </summary>
        /// <param name="staffId">The staff member ID</param>
        /// <returns>The total amount owed</returns>
        public async Task<decimal> GetTotalOwedToStaffAsync(Guid staffId)
        {
            return await Query()
                .Where(p => p.StaffId == staffId)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        }
    }
}
