using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_DASHBOARD.Models;

namespace API_DASHBOARD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleTypesController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public SaleTypesController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/SaleTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleType>>> GetSaleTypes()
        {
            return await _context.SaleTypes.ToListAsync();
        }

        // GET: api/SaleTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleType>> GetSaleType(int id)
        {
            var saleType = await _context.SaleTypes.FindAsync(id);

            if (saleType == null)
            {
                return NotFound();
            }

            return saleType;
        }

        // PUT: api/SaleTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleType(int id, SaleType saleType)
        {
            if (id != saleType.SaleTypeId)
            {
                return BadRequest();
            }

            _context.Entry(saleType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SaleTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SaleType>> PostSaleType(SaleType saleType)
        {
            _context.SaleTypes.Add(saleType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSaleType", new { id = saleType.SaleTypeId }, saleType);
        }

        // DELETE: api/SaleTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleType(int id)
        {
            var saleType = await _context.SaleTypes.FindAsync(id);
            if (saleType == null)
            {
                return NotFound();
            }

            _context.SaleTypes.Remove(saleType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleTypeExists(int id)
        {
            return _context.SaleTypes.Any(e => e.SaleTypeId == id);
        }
    }
}
