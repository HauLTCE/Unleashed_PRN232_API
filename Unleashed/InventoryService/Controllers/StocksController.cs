using InventoryService.DTOs.Stock;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        // GET: api/Stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(stocks);
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetStock(int id)
        {
            var stock = await _stockService.GetStockByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // PUT: api/Stocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, UpdateStockDto stockDto)
        {
            var updateResult = await _stockService.UpdateStockAsync(id, stockDto);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Stocks
        [HttpPost]
        public async Task<ActionResult<StockDto>> PostStock(CreateStockDto stockDto)
        {
            var createdStock = await _stockService.CreateStockAsync(stockDto);
            return CreatedAtAction(nameof(GetStock), new { id = createdStock.StockId }, createdStock);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var deleteResult = await _stockService.DeleteStockAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}