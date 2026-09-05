using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data.Repositories;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;

namespace SimplePubManager.Api.Controllers
{
    /// <summary>
    /// Controller for holiday management endpoints.
    /// Organization ID is resolved from the request context by TenantResolutionMiddleware.
    /// </summary>
    [ApiController]
    [Route("api/v1/holidays")]
    [Authorize]
    public class HolidaysController : ControllerBase
    {
        private readonly HolidayRepository _holidayRepository;
        private readonly UserRepository _userRepository;
        private readonly ILogger<HolidaysController> _logger;

        /// <summary>
        /// Initializes a new instance of the HolidaysController class.
        /// </summary>
        public HolidaysController(
            HolidayRepository holidayRepository,
            UserRepository userRepository,
            ILogger<HolidaysController> logger)
        {
            _holidayRepository = holidayRepository ?? throw new ArgumentNullException(nameof(holidayRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the organization ID from the request context (set by TenantResolutionMiddleware).
        /// </summary>
        private Guid GetOrganizationId()
        {
            if (HttpContext.Items.TryGetValue("OrganizationId", out var orgIdObj) && orgIdObj is Guid orgId)
            {
                return orgId;
            }
            throw new InvalidOperationException("Organization not found in request context");
        }

        /// <summary>
        /// Lists holidays for an organization with filtering.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of holidays</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<HolidayResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHolidays(
            Guid orgId,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var allHolidays = await _holidayRepository.GetAllAsync();
                var filtered = allHolidays.Where(h => h.OrganizationId == orgId);

                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<HolidayStatus>(status, ignoreCase: true, out var holidayStatus))
                {
                    filtered = filtered.Where(h => h.Status == holidayStatus);
                }

                var totalCount = filtered.Count();
                var holidays = filtered
                    .OrderByDescending(h => h.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<HolidayResponse>
                {
                    Items = holidays.Select(h => new HolidayResponse
                    {
                        Id = h.Id,
                        StaffId = h.StaffId,
                        StartDate = h.StartDate,
                        EndDate = h.EndDate,
                        Status = h.Status.ToString(),
                        Reason = h.Type,
                        CreatedAt = h.RequestedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<HolidayResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving holidays");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving holidays"
                        }
                    });
            }
        }

        /// <summary>
        /// Creates a new holiday request.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The holiday creation request</param>
        /// <returns>The created holiday request</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<HolidayResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateHolidayRequest request)
        {
            try
            {
                if (request == null || request.StartDate == default || request.EndDate == default)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Start date and end date are required"
                        }
                    });
                }

                if (request.EndDate < request.StartDate)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "End date must be after start date"
                        }
                    });
                }

                // Get current user ID from claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "User context not available"
                        }
                    });
                }

                var holiday = new SimplePubManager.Domain.Entities.Holiday
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = GetOrganizationId(),
                    StaffId = userId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = HolidayStatus.Pending,
                    Type = request.Reason ?? "vacation",
                    RequestedAt = DateTime.UtcNow
                };

                var createdHoliday = await _holidayRepository.AddAsync(holiday);

                return CreatedAtAction(nameof(GetHolidayById), new { id = createdHoliday.Id },
                    new ApiResponse<HolidayResponse>
                    {
                        Data = new HolidayResponse
                        {
                            Id = createdHoliday.Id,
                            StaffId = createdHoliday.StaffId,
                            StartDate = createdHoliday.StartDate,
                            EndDate = createdHoliday.EndDate,
                            Status = createdHoliday.Status.ToString(),
                            Reason = createdHoliday.Type,
                            CreatedAt = createdHoliday.RequestedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating holiday");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the holiday request"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific holiday request.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The holiday ID</param>
        /// <returns>The holiday details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HolidayResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHolidayById( Guid id)
        {
            try
            {
                var holiday = await _holidayRepository.GetByIdAsync(id);
                if (holiday == null || holiday.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "HOLIDAY_NOT_FOUND",
                            Message = "Holiday not found"
                        }
                    });
                }

                return Ok(new ApiResponse<HolidayResponse>
                {
                    Data = new HolidayResponse
                    {
                        Id = holiday.Id,
                        StaffId = holiday.StaffId,
                        StartDate = holiday.StartDate,
                        EndDate = holiday.EndDate,
                        Status = holiday.Status.ToString(),
                        Reason = holiday.Type,
                        CreatedAt = holiday.RequestedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving holiday");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the holiday"
                        }
                    });
            }
        }

        /// <summary>
        /// Approves a holiday request (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The holiday ID</param>
        /// <returns>The updated holiday</returns>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<HolidayResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveHoliday( Guid id)
        {
            try
            {
                var holiday = await _holidayRepository.GetByIdAsync(id);
                if (holiday == null || holiday.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "HOLIDAY_NOT_FOUND",
                            Message = "Holiday not found"
                        }
                    });
                }

                holiday.Status = HolidayStatus.Approved;
                holiday.ApprovedAt = DateTime.UtcNow;
                await _holidayRepository.UpdateAsync(holiday);

                return Ok(new ApiResponse<HolidayResponse>
                {
                    Data = new HolidayResponse
                    {
                        Id = holiday.Id,
                        StaffId = holiday.StaffId,
                        StartDate = holiday.StartDate,
                        EndDate = holiday.EndDate,
                        Status = holiday.Status.ToString(),
                        Reason = holiday.Type,
                        CreatedAt = holiday.RequestedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving holiday");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while approving the holiday"
                        }
                    });
            }
        }

        /// <summary>
        /// Rejects a holiday request (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The holiday ID</param>
        /// <returns>The updated holiday</returns>
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<HolidayResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RejectHoliday( Guid id)
        {
            try
            {
                var holiday = await _holidayRepository.GetByIdAsync(id);
                if (holiday == null || holiday.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "HOLIDAY_NOT_FOUND",
                            Message = "Holiday not found"
                        }
                    });
                }

                holiday.Status = HolidayStatus.Rejected;
                await _holidayRepository.UpdateAsync(holiday);

                return Ok(new ApiResponse<HolidayResponse>
                {
                    Data = new HolidayResponse
                    {
                        Id = holiday.Id,
                        StaffId = holiday.StaffId,
                        StartDate = holiday.StartDate,
                        EndDate = holiday.EndDate,
                        Status = holiday.Status.ToString(),
                        Reason = holiday.Type,
                        CreatedAt = holiday.RequestedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting holiday");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while rejecting the holiday"
                        }
                    });
            }
        }
    }
}
