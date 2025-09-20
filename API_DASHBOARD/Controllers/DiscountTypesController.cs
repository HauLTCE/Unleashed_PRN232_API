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
    public class DiscountTypesController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public DiscountTypesController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/DiscountTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountType>>> GetDiscountTypes()
        {
            return await _context.DiscountTypes.ToListAsync();
        }

        // GET: api/DiscountTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountType>> GetDiscountType(int id)
        {
            var discountType = await _context.DiscountTypes.FindAsync(id);

            if (discountType == null)
            {
                return NotFound();
            }

            return discountType;
        }

        // PUT: api/DiscountTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscountType(int id, DiscountType discountType)
        {
            if (id != discountType.DiscountTypeId)
            {
                return BadRequest();
            }

            _context.Entry(discountType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountTypeExists(id))
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

        // POST: api/DiscountTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DiscountType>> PostDiscountType(DiscountType discountType)
        {
            _context.DiscountTypes.Add(discountType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiscountType", new { id = discountType.DiscountTypeId }, discountType);
        }

        // DELETE: api/DiscountTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountType(int id)
        {
            var discountType = await _context.DiscountTypes.FindAsync(id);
            if (discountType == null)
            {
                return NotFound();
            }

            _context.DiscountTypes.Remove(discountType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscountTypeExists(int id)
        {
            return _context.DiscountTypes.Any(e => e.DiscountTypeId == id);
        }
    }
}
