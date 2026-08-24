using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplePubManager.Infrastructure.Data.Repositories;
using SimplePubManager.Infrastructure.Services;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;

namespace SimplePubManager.Api.Controllers
{
    /// <summary>
    /// Controller for shared device management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v1/organizations/{orgId}/devices")]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly DeviceAuthService _deviceAuthService;
        private readonly ILogger<DevicesController> _logger;

        /// <summary>
        /// Initializes a new instance of the DevicesController class.
        /// </summary>
        public DevicesController(
            DeviceRepository deviceRepository,
            DeviceAuthService deviceAuthService,
            ILogger<DevicesController> logger)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _deviceAuthService = deviceAuthService ?? throw new ArgumentNullException(nameof(deviceAuthService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists registered devices for an organization (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of devices</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DeviceResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDevices(
            Guid orgId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var allDevices = await _deviceRepository.GetAllAsync();
                var filtered = allDevices.Where(d => d.OrganizationId == orgId);

                var totalCount = filtered.Count();
                var devices = filtered
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<DeviceResponse>
                {
                    Items = devices.Select(d => new DeviceResponse
                    {
                        Id = d.Id,
                        DeviceId = d.DeviceId,
                        Name = d.Name,
                        Enabled = d.Enabled,
                        CreatedAt = d.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<DeviceResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving devices");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving devices"
                        }
                    });
            }
        }

        /// <summary>
        /// Registers a new shared device (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The device registration request</param>
        /// <returns>The registered device</returns>
        [HttpPost("register")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<DeviceResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterDevice(Guid orgId, [FromBody] RegisterDeviceRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.DeviceId) ||
                    string.IsNullOrWhiteSpace(request.DeviceKey) || string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Device ID, key, and name are required"
                        }
                    });
                }

                var device = await _deviceAuthService.RegisterDeviceAsync(orgId, request.DeviceId, request.DeviceKey, request.Name);

                if (device == null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "DEVICE_EXISTS",
                            Message = "A device with this ID already exists"
                        }
                    });
                }

                return CreatedAtAction(nameof(GetDevices), new { orgId },
                    new ApiResponse<DeviceResponse>
                    {
                        Data = new DeviceResponse
                        {
                            Id = device.Id,
                            DeviceId = device.DeviceId,
                            Name = device.Name,
                            Enabled = device.Enabled,
                            CreatedAt = device.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while registering the device"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates device settings (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The device ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated device</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<DeviceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateDevice(Guid orgId, Guid id, [FromBody] UpdateDeviceRequest request)
        {
            try
            {
                var device = await _deviceRepository.GetByIdAsync(id);
                if (device == null || device.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "DEVICE_NOT_FOUND",
                            Message = "Device not found"
                        }
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    device.Name = request.Name;
                }

                if (request.Enabled.HasValue)
                {
                    device.Enabled = request.Enabled.Value;
                }

                await _deviceRepository.UpdateAsync(device);

                return Ok(new ApiResponse<DeviceResponse>
                {
                    Data = new DeviceResponse
                    {
                        Id = device.Id,
                        DeviceId = device.DeviceId,
                        Name = device.Name,
                        Enabled = device.Enabled,
                        CreatedAt = device.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the device"
                        }
                    });
            }
        }
    }
}
