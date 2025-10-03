using InventoryService.DTOs.StockVariation;
using InventoryService.Services.Interfaces;
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
        public async Task<ActionResult<IEnumerable<StockVariationDto>>> GetStockVariations()
        {
            var stockVariations = await _stockVariationService.GetAllStockVariationsAsync();
            return Ok(stockVariations);
        }

        // GET: api/StockVariations/1/2
        [HttpGet("{stockId}/{variationId}")]
        public async Task<ActionResult<StockVariationDto>> GetStockVariation(int stockId, int variationId)
        {
            var stockVariation = await _stockVariationService.GetStockVariationByIdAsync(stockId, variationId);

            if (stockVariation == null)
            {
                return NotFound();
            }

            return Ok(stockVariation);
        }

        // PUT: api/StockVariations/1/2
        [HttpPut("{stockId}/{variationId}")]
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
        public async Task<IActionResult> DeleteStockVariation(int stockId, int variationId)
        {
            var deleteResult = await _stockVariationService.DeleteStockVariationAsync(stockId, variationId);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}