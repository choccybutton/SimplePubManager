using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplePubManager.Infrastructure.Data.Repositories;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;

namespace SimplePubManager.Api.Controllers
{
    /// <summary>
    /// Controller for area management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v1/organizations/{orgId}/areas")]
    [Authorize]
    public class AreasController : ControllerBase
    {
        private readonly AreaRepository _areaRepository;
        private readonly ILogger<AreasController> _logger;

        /// <summary>
        /// Initializes a new instance of the AreasController class.
        /// </summary>
        public AreasController(
            AreaRepository areaRepository,
            ILogger<AreasController> logger)
        {
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists work areas for an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of areas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AreaResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAreas(
            Guid orgId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var allAreas = await _areaRepository.GetAllAsync();
                var filtered = allAreas.Where(a => a.OrganizationId == orgId);

                var totalCount = filtered.Count();
                var areas = filtered
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<AreaResponse>
                {
                    Items = areas.Select(a => new AreaResponse
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        CreatedAt = a.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<AreaResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving areas");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving areas"
                        }
                    });
            }
        }

        /// <summary>
        /// Creates a new work area (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The area creation request</param>
        /// <returns>The created area</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<AreaResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateArea(Guid orgId, [FromBody] CreateAreaRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Area name is required"
                        }
                    });
                }

                var area = new SimplePubManager.Domain.Entities.Area
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                var createdArea = await _areaRepository.AddAsync(area);

                return CreatedAtAction(nameof(GetAreaById), new { orgId, id = createdArea.Id },
                    new ApiResponse<AreaResponse>
                    {
                        Data = new AreaResponse
                        {
                            Id = createdArea.Id,
                            Name = createdArea.Name,
                            Description = createdArea.Description,
                            CreatedAt = createdArea.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating area");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the area"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific work area.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The area ID</param>
        /// <returns>The area details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AreaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAreaById(Guid orgId, Guid id)
        {
            try
            {
                var area = await _areaRepository.GetByIdAsync(id);
                if (area == null || area.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "AREA_NOT_FOUND",
                            Message = "Area not found"
                        }
                    });
                }

                return Ok(new ApiResponse<AreaResponse>
                {
                    Data = new AreaResponse
                    {
                        Id = area.Id,
                        Name = area.Name,
                        Description = area.Description,
                        CreatedAt = area.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving area");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the area"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates a work area (manager only).
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The area ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated area</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<AreaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateArea(Guid orgId, Guid id, [FromBody] UpdateAreaRequest request)
        {
            try
            {
                var area = await _areaRepository.GetByIdAsync(id);
                if (area == null || area.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "AREA_NOT_FOUND",
                            Message = "Area not found"
                        }
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    area.Name = request.Name;
                }

                if (request.Description != null)
                {
                    area.Description = request.Description;
                }

                await _areaRepository.UpdateAsync(area);

                return Ok(new ApiResponse<AreaResponse>
                {
                    Data = new AreaResponse
                    {
                        Id = area.Id,
                        Name = area.Name,
                        Description = area.Description,
                        CreatedAt = area.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating area");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the area"
                        }
                    });
            }
        }
    }
}
