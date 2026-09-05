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
    /// Controller for staff management endpoints.
    /// Organization ID is resolved from the request context by TenantResolutionMiddleware.
    /// </summary>
    [ApiController]
    [Route("api/v1/staff")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly AuthService _authService;
        private readonly PasswordHashService _passwordHashService;
        private readonly ILogger<StaffController> _logger;

        /// <summary>
        /// Initializes a new instance of the StaffController class.
        /// </summary>
        public StaffController(
            UserRepository userRepository,
            AuthService authService,
            PasswordHashService passwordHashService,
            ILogger<StaffController> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
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
        /// Lists staff members for the organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of staff members</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StaffResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStaff(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var orgId = GetOrganizationId();
                var allStaff = await _userRepository.GetAllAsync();
                var filtered = allStaff.Where(u => u.OrganizationId == orgId);

                var totalCount = filtered.Count();
                var staff = filtered
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<StaffResponse>
                {
                    Items = staff.Select(u => new StaffResponse
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role.ToString(),
                        Status = u.Status.ToString(),
                        CreatedAt = u.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<StaffResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving staff"
                        }
                    });
            }
        }

        /// <summary>
        /// Adds a new staff member to the organization (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The staff creation request</param>
        /// <returns>The created staff member</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddStaff([FromBody] RegisterUserRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();
                if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Name, email, password, and role are required"
                        }
                    });
                }

                if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Invalid role specified"
                        }
                    });
                }

                var user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, orgId, role);

                if (user == null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "USER_EXISTS",
                            Message = "A user with this email already exists"
                        }
                    });
                }

                return CreatedAtAction(nameof(GetStaffById), new { id = user.Id },
                    new ApiResponse<StaffResponse>
                    {
                        Data = new StaffResponse
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Role = user.Role.ToString(),
                            Status = user.Status.ToString(),
                            CreatedAt = user.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding staff");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while adding the staff member"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific staff member.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The staff member ID</param>
        /// <returns>The staff member details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffById( Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "STAFF_NOT_FOUND",
                            Message = "Staff member not found"
                        }
                    });
                }

                return Ok(new ApiResponse<StaffResponse>
                {
                    Data = new StaffResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString(),
                        CreatedAt = user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the staff member"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates a staff member (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The staff member ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated staff member</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStaff( Guid id, [FromBody] UpdateStaffRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "STAFF_NOT_FOUND",
                            Message = "Staff member not found"
                        }
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    user.Name = request.Name;
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    user.Email = request.Email;
                }

                if (!string.IsNullOrWhiteSpace(request.Role) &&
                    Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
                {
                    user.Role = role;
                }

                if (!string.IsNullOrWhiteSpace(request.Status) &&
                    Enum.TryParse<UserStatus>(request.Status, ignoreCase: true, out var status))
                {
                    user.Status = status;
                }

                await _userRepository.UpdateAsync(user);

                return Ok(new ApiResponse<StaffResponse>
                {
                    Data = new StaffResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString(),
                        CreatedAt = user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the staff member"
                        }
                    });
            }
        }
    }
}
