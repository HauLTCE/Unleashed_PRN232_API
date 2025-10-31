using InventoryService.DTOs.Internal;
using InventoryService.DTOs.Transaction;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("bulk-import")]
        [Authorize(Roles = "ADMIN,STAFF")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostBulkTransactions([FromBody] StockTransactionDto importDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _transactionService.CreateBulkStockTransactionsAsync(importDto);

            if (success) return Ok(new { Message = "Bulk import of stock transactions successful." });

            return BadRequest(new { Message = "Bulk import of stock transactions failed. Please check the service logs for details." });
        }

        [HttpPost("reserve-order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReserveStock([FromBody] List<ProductVariationQuantityDto> items)
        {
            if (items == null || !items.Any())
            {
                return BadRequest("Item list cannot be empty.");
            }

            var success = await _transactionService.ReserveStockForOrderAsync(items);

            if (success) return Ok(new { Message = "Stock reserved successfully." });

            return BadRequest(new { Message = "Failed to reserve stock. Insufficient quantity may be the cause." });
        }

        [HttpPost("return-stock")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnStock([FromBody] List<ProductVariationQuantityDto> items)
        {
            if (items == null || !items.Any())
            {
                return BadRequest("Item list cannot be empty.");
            }

            var employeeIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            Guid? employeeId = employeeIdClaim != null ? Guid.Parse(employeeIdClaim.Value) : null;

            var success = await _transactionService.ReturnStockFromOrderAsync(items, employeeId);

            if (success) return Ok(new { Message = "Stock returned successfully." });

            return BadRequest(new { Message = "Failed to return stock." });
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<PaginatedResult<TransactionCardDTO>>> GetTransactions(
            [FromQuery] string? search,
            [FromQuery] string? dateFilter,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int size = 15)
        {
            var result = await _transactionService.GetTransactionsFilteredAsync(search, dateFilter, sort, page, size);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> PutTransaction(int id, UpdateTransactionDto transactionDto)
        {
            var updateResult = await _transactionService.UpdateTransactionAsync(id, transactionDto);
            if (!updateResult) return NotFound();
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<TransactionDto>> PostTransaction(CreateTransactionDto transactionDto)
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDto);
            if (createdTransaction == null)
            {
                return BadRequest("Could not create transaction.");
            }
            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var deleteResult = await _transactionService.DeleteTransactionAsync(id);
            if (!deleteResult) return NotFound();
            return NoContent();
        }
    }
}