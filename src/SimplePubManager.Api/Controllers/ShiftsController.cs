using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data.Repositories;
using SimplePubManager.Infrastructure.Services;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;

namespace SimplePubManager.Api.Controllers
{
    /// <summary>
    /// Controller for shift management endpoints.
    /// Organization ID is resolved from the request context by TenantResolutionMiddleware.
    /// </summary>
    [ApiController]
    [Route("api/v1/shifts")]
    [Authorize]
    public class ShiftsController : ControllerBase
    {
        private readonly ShiftRepository _shiftRepository;
        private readonly UserRepository _userRepository;
        private readonly AreaRepository _areaRepository;
        private readonly AuthService _authService;
        private readonly PaymentCalculationService _paymentCalculationService;
        private readonly ILogger<ShiftsController> _logger;

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
        /// Initializes a new instance of the ShiftsController class.
        /// </summary>
        public ShiftsController(
            ShiftRepository shiftRepository,
            UserRepository userRepository,
            AreaRepository areaRepository,
            AuthService authService,
            PaymentCalculationService paymentCalculationService,
            ILogger<ShiftsController> logger)
        {
            _shiftRepository = shiftRepository ?? throw new ArgumentNullException(nameof(shiftRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _paymentCalculationService = paymentCalculationService ?? throw new ArgumentNullException(nameof(paymentCalculationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists shifts for the organization with pagination and filtering.
        /// </summary>
        /// <param name="status">Optional shift status filter</param>
        /// <param name="staffId">Optional staff ID filter</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of shifts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ShiftResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetShifts(
            [FromQuery] string? status = null,
            [FromQuery] Guid? staffId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var orgId = GetOrganizationId();

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                ShiftStatus? statusFilter = null;
                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ShiftStatus>(status, ignoreCase: true, out var s))
                {
                    statusFilter = s;
                }

                IEnumerable<SimplePubManager.Domain.Entities.Shift> shifts;

                if (statusFilter.HasValue)
                {
                    shifts = await _shiftRepository.GetShiftsByOrganizationAndStatusAsync(orgId, statusFilter.Value, page, pageSize);
                }
                else
                {
                    shifts = (await _shiftRepository.GetAllAsync())
                        .Where(s => s.OrganizationId == orgId)
                        .OrderByDescending(s => s.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize);
                }

                if (staffId.HasValue)
                {
                    shifts = shifts.Where(s => s.StaffId == staffId.Value);
                }

                var response = new PaginatedResponse<ShiftResponse>
                {
                    Items = shifts.Select(s => new ShiftResponse
                    {
                        Id = s.Id,
                        StaffId = s.StaffId,
                        Type = s.Type.ToString(),
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Status = s.Status.ToString(),
                        AreaIds = s.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = s.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = (await _shiftRepository.GetAllAsync()).Count(s => s.OrganizationId == orgId)
                };

                return Ok(new ApiResponse<PaginatedResponse<ShiftResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shifts");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving shifts"
                        }
                    });
            }
        }

        /// <summary>
        /// Creates a new shift.
        /// </summary>
        /// <param name="request">The shift creation request</param>
        /// <returns>The created shift</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShift([FromBody] CreateShiftRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();

                if (request == null || request.StaffId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Staff ID, type, start time are required"
                        }
                    });
                }

                if (!Enum.TryParse<ShiftType>(request.Type, ignoreCase: true, out var shiftType))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Invalid shift type"
                        }
                    });
                }

                var staff = await _userRepository.GetByIdAsync(request.StaffId);
                if (staff == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "STAFF_NOT_FOUND",
                            Message = "Staff member not found"
                        }
                    });
                }

                var shift = new SimplePubManager.Domain.Entities.Shift
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    StaffId = request.StaffId,
                    Type = shiftType,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = ShiftStatus.Active,
                    CreatedBy = Guid.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                var createdShift = await _shiftRepository.AddAsync(shift);

                return CreatedAtAction(nameof(GetShiftById), new { id = createdShift.Id },
                    new ApiResponse<ShiftResponse>
                    {
                        Data = new ShiftResponse
                        {
                            Id = createdShift.Id,
                            StaffId = createdShift.StaffId,
                            Type = createdShift.Type.ToString(),
                            StartTime = createdShift.StartTime,
                            EndTime = createdShift.EndTime,
                            Status = createdShift.Status.ToString(),
                            AreaIds = new(),
                            CreatedAt = createdShift.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the shift"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <returns>The shift details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShiftById(Guid id)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shift");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the shift"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates a shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated shift</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShift(Guid id, [FromBody] UpdateShiftRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                if (request.EndTime.HasValue)
                {
                    shift.EndTime = request.EndTime;
                }

                if (!string.IsNullOrWhiteSpace(request.Status) &&
                    Enum.TryParse<ShiftStatus>(request.Status, ignoreCase: true, out var status))
                {
                    shift.Status = status;
                }

                await _shiftRepository.UpdateAsync(shift);

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the shift"
                        }
                    });
            }
        }

        /// <summary>
        /// Deletes/cancels a shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                shift.Status = ShiftStatus.Cancelled;
                await _shiftRepository.UpdateAsync(shift);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shift");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while deleting the shift"
                        }
                    });
            }
        }

        /// <summary>
        /// Records a clock-in for a shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <param name="request">The clock-in request</param>
        /// <returns>Updated shift details</returns>
        [HttpPost("{id}/clock-in")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClockIn(Guid id, [FromBody] ClockInRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                shift.Status = ShiftStatus.Active;
                await _shiftRepository.UpdateAsync(shift);

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking in");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while clocking in"
                        }
                    });
            }
        }

        /// <summary>
        /// Records a clock-out for a shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <param name="request">The clock-out request (unused, for consistency)</param>
        /// <returns>Updated shift details</returns>
        [HttpPost("{id}/clock-out")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClockOut(Guid id, [FromBody] ClockInRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                shift.EndTime = DateTime.UtcNow;
                shift.Status = ShiftStatus.Approved;
                await _shiftRepository.UpdateAsync(shift);

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking out");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while clocking out"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates areas assigned to a shift.
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <param name="request">The update request with area IDs</param>
        /// <returns>Updated shift details</returns>
        [HttpPut("{id}/areas")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShiftAreas(Guid id, [FromBody] UpdateShiftAreasRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetShiftWithAreasAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                // Clear existing areas and add new ones
                shift.Areas.Clear();
                foreach (var areaId in request.AreaIds ?? new())
                {
                    var area = await _areaRepository.GetByIdAsync(areaId);
                    if (area != null && area.OrganizationId == orgId)
                    {
                        shift.Areas.Add(new SimplePubManager.Domain.Entities.ShiftArea
                        {
                            ShiftId = shift.Id,
                            AreaId = areaId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                await _shiftRepository.UpdateAsync(shift);

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift areas");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating shift areas"
                        }
                    });
            }
        }

        /// <summary>
        /// Approves a shift (manager only).
        /// </summary>
        /// <param name="id">The shift ID</param>
        /// <returns>Updated shift details</returns>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<ShiftResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveShift(Guid id)
        {
            try
            {
                var orgId = GetOrganizationId();

                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null || shift.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "SHIFT_NOT_FOUND",
                            Message = "Shift not found"
                        }
                    });
                }

                shift.Status = ShiftStatus.Approved;
                await _shiftRepository.UpdateAsync(shift);

                return Ok(new ApiResponse<ShiftResponse>
                {
                    Data = new ShiftResponse
                    {
                        Id = shift.Id,
                        StaffId = shift.StaffId,
                        Type = shift.Type.ToString(),
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        Status = shift.Status.ToString(),
                        AreaIds = shift.Areas.Select(a => a.AreaId).ToList(),
                        CreatedAt = shift.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving shift");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while approving the shift"
                        }
                    });
            }
        }
    }
}
