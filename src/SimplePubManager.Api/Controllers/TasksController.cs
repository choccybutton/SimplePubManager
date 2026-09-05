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
    /// Controller for task management endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v1/organizations/{orgId}/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        private readonly UserRepository _userRepository;
        private readonly AreaRepository _areaRepository;
        private readonly ILogger<TasksController> _logger;

        /// <summary>
        /// Initializes a new instance of the TasksController class.
        /// </summary>
        public TasksController(
            TaskRepository taskRepository,
            UserRepository userRepository,
            AreaRepository areaRepository,
            ILogger<TasksController> logger)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _areaRepository = areaRepository ?? throw new ArgumentNullException(nameof(areaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lists tasks for an organization with filtering and pagination.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="assignedTo">Optional assignee ID filter</param>
        /// <param name="areaId">Optional area ID filter</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of tasks</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TaskResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTasks(
            Guid orgId,
            [FromQuery] string? status = null,
            [FromQuery] Guid? assignedTo = null,
            [FromQuery] Guid? areaId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var allTasks = await _taskRepository.GetAllAsync();
                var filtered = allTasks.Where(t => t.OrganizationId == orgId);

                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SimplePubManager.Domain.Enums.TaskStatus>(status, ignoreCase: true, out var taskStatus))
                {
                    filtered = filtered.Where(t => t.Status == taskStatus);
                }

                if (assignedTo.HasValue)
                {
                    filtered = filtered.Where(t => t.AssignedToUserId == assignedTo.Value);
                }

                if (areaId.HasValue)
                {
                    filtered = filtered.Where(t => t.AssignedToAreaId == areaId.Value);
                }

                var totalCount = filtered.Count();
                var tasks = filtered
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<TaskResponse>
                {
                    Items = tasks.Select(t => new TaskResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description ?? string.Empty,
                        AssignedToId = t.AssignedToUserId,
                        Status = t.Status.ToString(),
                        AreaId = t.AssignedToAreaId,
                        CreatedAt = t.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<TaskResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving tasks"
                        }
                    });
            }
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The task creation request</param>
        /// <returns>The created task</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask(Guid orgId, [FromBody] CreateTaskRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Title is required"
                        }
                    });
                }

                var task = new SimplePubManager.Domain.Entities.Models.Task
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    Title = request.Title,
                    Description = request.Description ?? string.Empty,
                    AssignedToUserId = request.AssignedToId,
                    AssignedToAreaId = request.AreaId,
                    Status = SimplePubManager.Domain.Enums.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7)
                };

                var createdTask = await _taskRepository.AddAsync(task);

                return CreatedAtAction(nameof(GetTaskById), new { orgId, id = createdTask.Id },
                    new ApiResponse<TaskResponse>
                    {
                        Data = new TaskResponse
                        {
                            Id = createdTask.Id,
                            Title = createdTask.Title,
                            Description = createdTask.Description ?? string.Empty,
                            AssignedToId = createdTask.AssignedToUserId,
                            Status = createdTask.Status.ToString(),
                            AreaId = createdTask.AssignedToAreaId,
                            CreatedAt = createdTask.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the task"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific task.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The task ID</param>
        /// <returns>The task details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(Guid orgId, Guid id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null || task.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "TASK_NOT_FOUND",
                            Message = "Task not found"
                        }
                    });
                }

                return Ok(new ApiResponse<TaskResponse>
                {
                    Data = new TaskResponse
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description ?? string.Empty,
                        AssignedToId = task.AssignedToUserId,
                        Status = task.Status.ToString(),
                        AreaId = task.AssignedToAreaId,
                        CreatedAt = task.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the task"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates a task.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The task ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated task</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(Guid orgId, Guid id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null || task.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "TASK_NOT_FOUND",
                            Message = "Task not found"
                        }
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    task.Title = request.Title;
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    task.Description = request.Description;
                }

                if (request.AssignedToId.HasValue)
                {
                    task.AssignedToUserId = request.AssignedToId;
                }

                if (!string.IsNullOrWhiteSpace(request.Status) &&
                    Enum.TryParse<SimplePubManager.Domain.Enums.TaskStatus>(request.Status, ignoreCase: true, out var status))
                {
                    task.Status = status;
                }

                await _taskRepository.UpdateAsync(task);

                return Ok(new ApiResponse<TaskResponse>
                {
                    Data = new TaskResponse
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description ?? string.Empty,
                        AssignedToId = task.AssignedToUserId,
                        Status = task.Status.ToString(),
                        AreaId = task.AssignedToAreaId,
                        CreatedAt = task.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the task"
                        }
                    });
            }
        }

        /// <summary>
        /// Marks a task as complete.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The task ID</param>
        /// <returns>The updated task</returns>
        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteTask(Guid orgId, Guid id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null || task.OrganizationId != orgId)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "TASK_NOT_FOUND",
                            Message = "Task not found"
                        }
                    });
                }

                task.Status = SimplePubManager.Domain.Enums.TaskStatus.Completed;
                task.CompletedAt = DateTime.UtcNow;
                await _taskRepository.UpdateAsync(task);

                return Ok(new ApiResponse<TaskResponse>
                {
                    Data = new TaskResponse
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description ?? string.Empty,
                        AssignedToId = task.AssignedToUserId,
                        Status = task.Status.ToString(),
                        AreaId = task.AssignedToAreaId,
                        CreatedAt = task.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while completing the task"
                        }
                    });
            }
        }
    }
}
