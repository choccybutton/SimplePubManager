using SimplePubManager.Domain.Entities;
using SimplePubManager.Infrastructure.Data.Repositories;

namespace SimplePubManager.Infrastructure.Services
{
    /// <summary>
    /// Service for device authentication and session management.
    /// </summary>
    public class DeviceAuthService
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly UserRepository _userRepository;
        private readonly PasswordHashService _passwordHashService;
        private readonly JwtTokenService _jwtTokenService;
        private const int DeviceSessionTimeoutMinutes = 15;

        /// <summary>
        /// Initializes a new instance of the DeviceAuthService class.
        /// </summary>
        /// <param name="deviceRepository">The device repository</param>
        /// <param name="userRepository">The user repository</param>
        /// <param name="passwordHashService">The password hash service</param>
        /// <param name="jwtTokenService">The JWT token service</param>
        public DeviceAuthService(
            DeviceRepository deviceRepository,
            UserRepository userRepository,
            PasswordHashService passwordHashService,
            JwtTokenService jwtTokenService)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        /// <summary>
        /// Validates a device's existence and enabled status using its ID and key.
        /// </summary>
        /// <param name="deviceId">The device ID (string identifier)</param>
        /// <param name="deviceKey">The device key to verify</param>
        /// <returns>A tuple containing validation result and error message if invalid</returns>
        public async Task<(bool valid, string? error)> ValidateDeviceAsync(string deviceId, string deviceKey)
        {
            if (string.IsNullOrWhiteSpace(deviceId) || string.IsNullOrWhiteSpace(deviceKey))
            {
                return (false, "Device ID and key are required");
            }

            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);

            if (device == null)
            {
                return (false, "Device not found");
            }

            if (!device.Enabled)
            {
                return (false, "Device is not enabled");
            }

            // Verify device key
            if (!_passwordHashService.VerifyPassword(deviceKey, device.DeviceKeyHash))
            {
                return (false, "Invalid device key");
            }

            return (true, null);
        }

        /// <summary>
        /// Creates a device session for a user using their PIN for quick-swap authentication.
        /// </summary>
        /// <param name="deviceId">The ID of the device (Guid)</param>
        /// <param name="userId">The ID of the user logging in</param>
        /// <param name="pin">The user's PIN for quick-swap authentication</param>
        /// <returns>A tuple containing the session token and error message if unsuccessful</returns>
        public async Task<(string? sessionToken, string? error)> PinQuickSwapAsync(Guid deviceId, Guid userId, string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                return (null, "PIN is required");
            }

            // Verify device exists and is enabled
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null || !device.Enabled)
            {
                return (null, "Device not found or disabled");
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (null, "User not found");
            }

            // Verify PIN - Find user's PIN for this device (or any PIN if no device restriction)
            var userPin = user.Pins.FirstOrDefault(p =>
                _passwordHashService.VerifyPassword(pin, p.PinHash) &&
                (p.DeviceRestrictionId == null || p.DeviceRestrictionId == deviceId));

            if (userPin == null)
            {
                return (null, "Invalid PIN");
            }

            // Generate session token
            var sessionToken = _jwtTokenService.GenerateToken(user, user.OrganizationId);

            return (sessionToken, null);
        }

        /// <summary>
        /// Validates a device session token and checks if it has expired.
        /// </summary>
        /// <param name="sessionToken">The session token to validate</param>
        /// <returns>A tuple containing validation result and the user ID if valid</returns>
        public async Task<(bool valid, Guid? userId)> ValidateDeviceSessionAsync(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return (false, null);
            }

            var principal = _jwtTokenService.ValidateToken(sessionToken);
            if (principal == null)
            {
                return (false, null);
            }

            // Extract user ID from claims
            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return (false, null);
            }

            // Verify user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, null);
            }

            return (true, userId);
        }

        /// <summary>
        /// Registers a new device in the system.
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <param name="deviceId">The human-readable device identifier</param>
        /// <param name="deviceKey">The device key for authentication</param>
        /// <param name="name">The display name for the device</param>
        /// <returns>The created device if successful; otherwise null</returns>
        public async Task<Device?> RegisterDeviceAsync(Guid organizationId, string deviceId, string deviceKey, string name)
        {
            if (string.IsNullOrWhiteSpace(deviceId) || string.IsNullOrWhiteSpace(deviceKey) || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            // Check if device ID already exists
            var existingDevice = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (existingDevice != null)
            {
                return null;
            }

            // Hash device key
            var deviceKeyHash = _passwordHashService.HashPassword(deviceKey);

            // Create device
            var device = new Device
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                DeviceId = deviceId,
                DeviceKeyHash = deviceKeyHash,
                Name = name,
                Enabled = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdDevice = await _deviceRepository.AddAsync(device);
            return createdDevice;
        }
    }
}
