using InventoryService.DTOs.TransactionType;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypesController : ControllerBase
    {
        private readonly ITransactionTypeService _transactionTypeService;

        public TransactionTypesController(ITransactionTypeService transactionTypeService)
        {
            _transactionTypeService = transactionTypeService;
        }

        // GET: api/TransactionTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionTypeDto>>> GetTransactionTypes()
        {
            var transactionTypes = await _transactionTypeService.GetAllTransactionTypesAsync();
            return Ok(transactionTypes);
        }

        // GET: api/TransactionTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionTypeDto>> GetTransactionType(int id)
        {
            var transactionType = await _transactionTypeService.GetTransactionTypeByIdAsync(id);

            if (transactionType == null)
            {
                return NotFound();
            }

            return Ok(transactionType);
        }

        // PUT: api/TransactionTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionType(int id, UpdateTransactionTypeDto transactionTypeDto)
        {
            var updateResult = await _transactionTypeService.UpdateTransactionTypeAsync(id, transactionTypeDto);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TransactionTypes
        [HttpPost]
        public async Task<ActionResult<TransactionTypeDto>> PostTransactionType(CreateTransactionTypeDto transactionTypeDto)
        {
            var createdTransactionType = await _transactionTypeService.CreateTransactionTypeAsync(transactionTypeDto);

            return CreatedAtAction(nameof(GetTransactionType),
                new { id = createdTransactionType.TransactionTypeId },
                createdTransactionType);
        }

        // DELETE: api/TransactionTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionType(int id)
        {
            var deleteResult = await _transactionTypeService.DeleteTransactionTypeAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}