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
    /// Controller for payment and billing endpoints.
    /// Organization ID is resolved from the request context by TenantResolutionMiddleware.
    /// </summary>
    [ApiController]
    [Route("api/v1/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly BillRepository _billRepository;
        private readonly ILogger<PaymentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the PaymentsController class.
        /// </summary>
        public PaymentsController(
            PaymentRepository paymentRepository,
            BillRepository billRepository,
            ILogger<PaymentsController> logger)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
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
        /// Lists bills for an organization with filtering.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of bills</returns>
        [HttpGet("bills")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<BillResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBills(
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

                var allBills = await _billRepository.GetAllAsync();
                var filtered = allBills.Where(b => b.OrganizationId == orgId);

                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BillStatus>(status, ignoreCase: true, out var billStatus))
                {
                    filtered = filtered.Where(b => b.Status == billStatus);
                }

                var totalCount = filtered.Count();
                var bills = filtered
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<BillResponse>
                {
                    Items = bills.Select(b => new BillResponse
                    {
                        Id = b.Id,
                        Description = b.Description,
                        Amount = b.Amount,
                        DueDate = b.DueDate,
                        Status = b.Status.ToString(),
                        CreatedAt = b.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<BillResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving bills"
                        }
                    });
            }
        }

        /// <summary>
        /// Creates a new bill.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The bill creation request</param>
        /// <returns>The created bill</returns>
        [HttpPost("bills")]
        [ProducesResponseType(typeof(ApiResponse<BillResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0 || request.DueDate == default)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Amount and due date are required"
                        }
                    });
                }

                var bill = new SimplePubManager.Domain.Entities.Bill
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = GetOrganizationId(),
                    Description = request.Description ?? string.Empty,
                    Amount = request.Amount,
                    DueDate = request.DueDate,
                    Status = BillStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                var createdBill = await _billRepository.AddAsync(bill);

                return CreatedAtAction(nameof(GetBillById), new { id = createdBill.Id },
                    new ApiResponse<BillResponse>
                    {
                        Data = new BillResponse
                        {
                            Id = createdBill.Id,
                            Description = createdBill.Description,
                            Amount = createdBill.Amount,
                            DueDate = createdBill.DueDate,
                            Status = createdBill.Status.ToString(),
                            CreatedAt = createdBill.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the bill"
                        }
                    });
            }
        }

        /// <summary>
        /// Gets details for a specific bill.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The bill ID</param>
        /// <returns>The bill details</returns>
        [HttpGet("bills/{id}")]
        [ProducesResponseType(typeof(ApiResponse<BillResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBillById( Guid id)
        {
            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null || bill.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "BILL_NOT_FOUND",
                            Message = "Bill not found"
                        }
                    });
                }

                return Ok(new ApiResponse<BillResponse>
                {
                    Data = new BillResponse
                    {
                        Id = bill.Id,
                        Description = bill.Description,
                        Amount = bill.Amount,
                        DueDate = bill.DueDate,
                        Status = bill.Status.ToString(),
                        CreatedAt = bill.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving the bill"
                        }
                    });
            }
        }

        /// <summary>
        /// Updates a bill.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="id">The bill ID</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated bill</returns>
        [HttpPut("bills/{id}")]
        [ProducesResponseType(typeof(ApiResponse<BillResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBill( Guid id, [FromBody] CreateBillRequest request)
        {
            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null || bill.OrganizationId != GetOrganizationId())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "BILL_NOT_FOUND",
                            Message = "Bill not found"
                        }
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    bill.Description = request.Description;
                }

                if (request.Amount > 0)
                {
                    bill.Amount = request.Amount;
                }

                if (request.DueDate != default)
                {
                    bill.DueDate = request.DueDate;
                }

                await _billRepository.UpdateAsync(bill);

                return Ok(new ApiResponse<BillResponse>
                {
                    Data = new BillResponse
                    {
                        Id = bill.Id,
                        Description = bill.Description,
                        Amount = bill.Amount,
                        DueDate = bill.DueDate,
                        Status = bill.Status.ToString(),
                        CreatedAt = bill.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while updating the bill"
                        }
                    });
            }
        }

        /// <summary>
        /// Lists payments for an organization.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Items per page (default 20)</param>
        /// <returns>Paginated list of payments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<PaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPayments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var orgId = GetOrganizationId();
                var allPayments = await _paymentRepository.GetAllAsync();
                var filtered = allPayments.Where(p => p.OrganizationId == orgId);

                var totalCount = filtered.Count();
                var payments = filtered
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<PaymentResponse>
                {
                    Items = payments.Select(p => new PaymentResponse
                    {
                        Id = p.Id,
                        StaffId = p.StaffId,
                        Amount = p.Amount,
                        Type = p.Type.ToString(),
                        RelatedShiftId = p.RelatedShiftId,
                        Date = p.Date,
                        CreatedAt = p.CreatedAt
                    }),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Ok(new ApiResponse<PaginatedResponse<PaymentResponse>>
                {
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while retrieving payments"
                        }
                    });
            }
        }

        /// <summary>
        /// Records a new payment.
        /// </summary>
        /// <param name="orgId">The organization ID</param>
        /// <param name="request">The payment creation request</param>
        /// <returns>The created payment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var orgId = GetOrganizationId();
                if (request == null || request.Amount <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "VALIDATION_ERROR",
                            Message = "Amount is required"
                        }
                    });
                }

                var payment = new SimplePubManager.Domain.Entities.Payment
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    StaffId = request.StaffId,
                    Amount = request.Amount,
                    Type = PaymentType.ShiftPayment,
                    RelatedShiftId = request.RelatedShiftId,
                    Date = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPayment = await _paymentRepository.AddAsync(payment);

                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<PaymentResponse>
                    {
                        Data = new PaymentResponse
                        {
                            Id = createdPayment.Id,
                            StaffId = createdPayment.StaffId,
                            Amount = createdPayment.Amount,
                            Type = createdPayment.Type.ToString(),
                            RelatedShiftId = createdPayment.RelatedShiftId,
                            Date = createdPayment.Date,
                            CreatedAt = createdPayment.CreatedAt
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Error = new ApiError
                        {
                            Code = "INTERNAL_ERROR",
                            Message = "An error occurred while creating the payment"
                        }
                    });
            }
        }
    }
}
