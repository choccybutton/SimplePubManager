using Microsoft.EntityFrameworkCore;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data;

namespace SimplePubManager.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for device-related data operations.
    /// </summary>
    public class DeviceRepository : BaseRepository<Device>
    {
        /// <summary>
        /// Initializes a new instance of the DeviceRepository class.
        /// </summary>
        /// <param name="context">The application database context</param>
        public DeviceRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a device by its device ID (string identifier).
        /// </summary>
        /// <param name="deviceId">The device ID (string identifier)</param>
        /// <returns>The device if found; otherwise null</returns>
        public async Task<Device?> GetDeviceByIdAsync(string deviceId)
        {
            return await Query()
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        /// <summary>
        /// Retrieves all enabled devices in an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <returns>An enumerable collection of enabled devices</returns>
        public async Task<IEnumerable<Device>> GetEnabledDevicesAsync(Guid orgId)
        {
            return await Query()
                .Where(d => d.OrganizationId == orgId && d.Enabled)
                .OrderBy(d => d.DeviceId)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a device is enabled.
        /// </summary>
        /// <param name="deviceId">The device ID (Guid)</param>
        /// <returns>True if the device is enabled; otherwise false</returns>
        public async Task<bool> CheckDeviceEnabledAsync(Guid deviceId)
        {
            return await Query()
                .Where(d => d.Id == deviceId)
                .AnyAsync(d => d.Enabled);
        }
    }
}
