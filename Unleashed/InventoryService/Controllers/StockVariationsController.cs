using InventoryService.DTOs.External;
using InventoryService.DTOs.StockVariation;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockVariationsController : ControllerBase
    {
        private readonly IStockVariationService _stockVariationService;

        public StockVariationsController(IStockVariationService stockVariationService)
        {
            _stockVariationService = stockVariationService;
        }

        // GET: api/StockVariations
        [HttpGet]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<IEnumerable<StockVariationDto>>> GetStockVariations()
        {
            var stockVariations = await _stockVariationService.GetAllStockVariationsAsync();
            return Ok(stockVariations);
        }

        // GET: api/StockVariations/1/2
        [HttpGet("{stockId}/{variationId}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<StockVariationDto>> GetStockVariation(int stockId, int variationId)
        {
            var stockVariation = await _stockVariationService.GetStockVariationByIdAsync(stockId, variationId);

            if (stockVariation == null)
            {
                return NotFound();
            }

            return Ok(stockVariation);
        }

        // GET: api/StockVariations/get-stock-by-variation/1
        [HttpGet("get-stock-by-variation/{variationId}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<Inventory_OrderDto>> GetStockByVariationId(int variationId)
        {
            var stockVariation = await _stockVariationService.GetStockByVariationIdAsync(variationId);

            if (stockVariation == null)
            {
                return NotFound();
            }

            return Ok(stockVariation);
        }

        [HttpPost("get-stock-by-ids")]
        public async Task<ActionResult<IEnumerable<Inventory_OrderDto>>> GetStockByIds([FromBody] IEnumerable<int> variationIds)
        {
            // 1. Check if the input list is empty
            if (variationIds == null || !variationIds.Any())
            {
                return Ok(Enumerable.Empty<Inventory_OrderDto>());
            }

            // 2. Delegate the batch logic to the service layer
            var stockVariations = await _stockVariationService.GetStockByVariationIdsAsync([.. variationIds]);

            // 3. Return the list of stock levels
            return Ok(stockVariations);
        }

        [HttpPut("decrease-stocks")]
        public async Task<IActionResult> DecreaseStocksAsync([FromBody] List<Order_InventoryDto> orderList)
        {
            if (orderList == null || orderList.Count == 0)
                return BadRequest(new { message = "Order list cannot be empty." });

            try
            {
                await _stockVariationService.DecreaseStocksAsync(orderList);
                return NoContent(); // 204: successful but no response body
            }
            catch (InvalidOperationException ex)
            { 
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) // unexpected server error
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }



        // PUT: api/StockVariations/1/2
        [HttpPut("{stockId}/{variationId}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> PutStockVariation(int stockId, int variationId, UpdateStockVariationDto stockVariationDto)
        {
            var updateResult = await _stockVariationService.UpdateStockVariationAsync(stockId, variationId, stockVariationDto);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/StockVariations
        [HttpPost]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<StockVariationDto>> PostStockVariation(CreateStockVariationDto stockVariationDto)
        {
            var createdStockVariation = await _stockVariationService.CreateStockVariationAsync(stockVariationDto);

            if (createdStockVariation == null)
            {
                return Conflict("A record with this StockId and VariationId already exists.");
            }

            return CreatedAtAction(nameof(GetStockVariation),
                new { stockId = createdStockVariation.StockId, variationId = createdStockVariation.VariationId },
                createdStockVariation);
        }

        // DELETE: api/StockVariations/1/2
        [HttpDelete("{stockId}/{variationId}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> DeleteStockVariation(int stockId, int variationId)
        {
            var deleteResult = await _stockVariationService.DeleteStockVariationAsync(stockId, variationId);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/StockVariations/by-stock/1
        [HttpGet("by-stock/{stockId}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<IEnumerable<StockVariationDto>>> GetStockVariationsByStockId(int stockId)
        {
            var stockVariations = await _stockVariationService.GetStockVariationsByStockIdAsync(stockId);
            if (stockVariations == null || !stockVariations.Any())
            {
                return Ok(Enumerable.Empty<StockVariationDto>());
            }
            return Ok(stockVariations);
        }

    }
}