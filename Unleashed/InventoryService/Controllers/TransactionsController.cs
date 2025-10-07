using InventoryService.DTOs.Internal;
using InventoryService.DTOs.Transaction;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            _logger.LogInformation(
                "Received bulk stock import request for Stock ID {StockId} and Provider ID {ProviderId}.",
                importDto.StockId, importDto.ProviderId);

            var success = await _transactionService.CreateBulkStockTransactionsAsync(importDto);

            if (success) return Ok(new { Message = "Bulk import of stock transactions successful." });
            
            return BadRequest(new { Message = "Bulk import of stock transactions failed. Please check the service logs for details." });
        }

        // GET: api/Transactions
        [HttpGet]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> PutTransaction(int id, UpdateTransactionDto transactionDto)
        {
            var updateResult = await _transactionService.UpdateTransactionAsync(id, transactionDto);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Transactions
        [HttpPost]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<TransactionDto>> PostTransaction(CreateTransactionDto transactionDto)
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDto);

            if (createdTransaction == null)
            {
                // Could be due to various reasons, e.g., not enough stock, stock variation not found
                return BadRequest("Could not create transaction. Please check stock levels and product details.");
            }

            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.TransactionId }, createdTransaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var deleteResult = await _transactionService.DeleteTransactionAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}