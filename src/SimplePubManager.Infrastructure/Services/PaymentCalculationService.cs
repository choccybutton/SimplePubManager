using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;
using SimplePubManager.Infrastructure.Data.Repositories;

namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for calculating shift payments and managing payment records.
    /// </summary>
    public class PaymentCalculationService
    {
        private readonly ShiftRepository _shiftRepository;
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PaymentCalculationService class.
        /// </summary>
        /// <param name="shiftRepository">The shift repository</param>
        /// <param name="userRepository">The user repository</param>
        /// <param name="context">The application database context</param>
        public PaymentCalculationService(
            ShiftRepository shiftRepository,
            UserRepository userRepository,
            AppDbContext context)
        {
            _shiftRepository = shiftRepository ?? throw new ArgumentNullException(nameof(shiftRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Calculates the total hours worked in a shift based on shift logs.
        /// </summary>
        /// <param name="shiftId">The shift ID</param>
        /// <returns>The total hours worked</returns>
        public async Task<decimal> CalculateShiftHoursAsync(Guid shiftId)
        {
            var shift = await _shiftRepository.GetByIdAsync(shiftId);
            if (shift == null)
            {
                return 0m;
            }

            var shiftLogs = await _context.Set<ShiftLog>()
                .Where(sl => sl.ShiftId == shiftId)
                .ToListAsync();

            decimal totalHours = 0m;

            foreach (var log in shiftLogs)
            {
                if (log.ClockOutTime.HasValue)
                {
                    var duration = log.ClockOutTime.Value - log.ClockInTime;
                    totalHours += (decimal)duration.TotalHours;
                }
                else
                {
                    // If not clocked out, calculate from clock in to now
                    var duration = DateTime.UtcNow - log.ClockInTime;
                    totalHours += (decimal)duration.TotalHours;
                }
            }

            return totalHours;
        }

        /// <summary>
        /// Calculates the payment amount for a shift based on hours worked and hourly rate.
        /// </summary>
        /// <param name="shiftId">The shift ID</param>
        /// <param name="hourlyRate">The hourly rate for the shift</param>
        /// <returns>The total payment amount</returns>
        public async Task<decimal> CalculateShiftPaymentAsync(Guid shiftId, decimal hourlyRate)
        {
            if (hourlyRate < 0)
            {
                return 0m;
            }

            var hours = await CalculateShiftHoursAsync(shiftId);
            return hours * hourlyRate;
        }

        /// <summary>
        /// Processes all pending approved shifts without payments and creates payment records.
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A list of created payment records</returns>
        public async Task<List<ShiftPayment>> ProcessPendingShiftsAsync(Guid organizationId)
        {
            var createdPayments = new List<ShiftPayment>();

            // Get all approved shifts
            var shifts = await _shiftRepository.GetShiftsByOrganizationAndStatusAsync(
                organizationId, ShiftStatus.Approved, 1, int.MaxValue);

            foreach (var shift in shifts)
            {
                // Check if payment already exists
                var existingPayment = await _context.Set<ShiftPayment>()
                    .FirstOrDefaultAsync(p => p.ShiftId == shift.Id);

                if (existingPayment != null)
                {
                    continue; // Skip if payment already exists
                }

                // Get staff member to determine hourly rate if needed
                var staff = await _userRepository.GetByIdAsync(shift.StaffId);
                if (staff == null)
                {
                    continue;
                }

                // Calculate hours and payment
                var hoursWorked = await CalculateShiftHoursAsync(shift.Id);

                // For now, use a default hourly rate of 0 (should be set from organization or shift settings)
                // This should be updated with proper rate calculation
                decimal hourlyRate = 0m;
                decimal amount = hoursWorked * hourlyRate;

                // Create payment record
                var payment = new ShiftPayment
                {
                    Id = Guid.NewGuid(),
                    ShiftId = shift.Id,
                    HourlyRate = hourlyRate,
                    HoursWorked = hoursWorked,
                    Amount = amount,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Set<ShiftPayment>().AddAsync(payment);
                createdPayments.Add(payment);
            }

            // Save all changes
            if (createdPayments.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return createdPayments;
        }

        /// <summary>
        /// Calculates the total amount owed to a user for all unpaid shifts.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The total amount owed</returns>
        public async Task<decimal> GetTotalOwedAsync(Guid userId)
        {
            var totalOwed = await _context.Set<ShiftPayment>()
                .Where(p => p.Shift!.StaffId == userId && p.Status != PaymentStatus.Paid)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            return totalOwed;
        }
    }
}
