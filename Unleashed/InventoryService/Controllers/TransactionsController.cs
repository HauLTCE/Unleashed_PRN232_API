using InventoryService.DTOs.Transaction;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
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