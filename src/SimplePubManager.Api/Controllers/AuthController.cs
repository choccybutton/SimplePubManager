using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplePubManager.Infrastructure.Services;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;

namespace SimplePubManager.Api.Controllers
{
    /// <summary>
    /// Controller for user authentication and device login endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DeviceAuthService _deviceAuthService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController class.
        /// </summary>
        public AuthController(
            AuthService authService,
            DeviceAuthService deviceAuthService,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _deviceAuthService = deviceAuthService ?? throw new ArgumentNullException(nameof(deviceAuthService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Authenticates a user with email and password and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request containing email and password</param>
        /// <returns>Authentication response with token and user details</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Email and password are required"
                        }
                    });
                }

                var (token, user) = await _authService.LoginAsync(request.Email, request.Password, request.OrganizationId);

                if (token == null || user == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "AUTH_FAILED",
                            Message = "Invalid email or password"
                        }
                    });
                }

                return Ok(new ApiResponse<AuthResponse>
                {
                    Data = new AuthResponse
                    {
                        Token = token,
                        UserId = user.Id,
                        UserName = user.Name,
                        Email = user.Email,
                        Role = user.Role.ToString(),
                        OrganizationId = user.OrganizationId
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred during authentication"
                        }
                    });
            }
        }

        /// <summary>
        /// Authenticates a device using device ID and key.
        /// </summary>
        /// <param name="request">The device login request</param>
        /// <returns>Device session token</returns>
        [HttpPost("device-login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeviceLogin([FromBody] DeviceLoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.DeviceId) || string.IsNullOrWhiteSpace(request.DeviceKey))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Device ID and key are required"
                        }
                    });
                }

                var (valid, error) = await _deviceAuthService.ValidateDeviceAsync(request.DeviceId, request.DeviceKey);

                if (!valid)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "DEVICE_AUTH_FAILED",
                            Message = error ?? "Device authentication failed"
                        }
                    });
                }

                return Ok(new ApiResponse<AuthResponse>
                {
                    Data = new AuthResponse
                    {
                        Token = request.DeviceId,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during device login");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred during device authentication"
                        }
                    });
            }
        }

        /// <summary>
        /// Performs a quick-swap login on a shared device using PIN authentication.
        /// </summary>
        /// <param name="request">The quick-swap request containing device ID, user ID, and PIN</param>
        /// <returns>Device session token</returns>
        [HttpPost("quick-swap")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> QuickSwap([FromBody] QuickSwapRequest request)
        {
            try
            {
                if (request == null || request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.Pin))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Device ID, user ID, and PIN are required"
                        }
                    });
                }

                // Convert string device ID to GUID
                var deviceId = request.DeviceId;
                if (!Guid.TryParse(deviceId, out var deviceGuid))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Invalid device ID format"
                        }
                    });
                }

                var (sessionToken, error) = await _deviceAuthService.PinQuickSwapAsync(deviceGuid, request.UserId, request.Pin);

                if (sessionToken == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "QUICK_SWAP_FAILED",
                            Message = error ?? "Quick-swap authentication failed"
                        }
                    });
                }

                return Ok(new ApiResponse<AuthResponse>
                {
                    Data = new AuthResponse
                    {
                        Token = sessionToken,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during quick-swap");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred during quick-swap authentication"
                        }
                    });
            }
        }

        /// <summary>
        /// Registers a new user in the system (admin only).
        /// </summary>
        /// <param name="request">The registration request</param>
        /// <returns>The created user information</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
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

                // Parse role
                if (!Enum.TryParse<SimplePubManager.Domain.Enums.UserRole>(request.Role, ignoreCase: true, out var role))
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

                var user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, Guid.NewGuid(), role);

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

                return CreatedAtAction(nameof(Register), new StaffResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred during registration"
                        }
                    });
            }
        }
    }
}
