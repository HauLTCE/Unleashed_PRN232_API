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
    public class VariationsController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public VariationsController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/Variations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Variation>>> GetVariations()
        {
            return await _context.Variations.ToListAsync();
        }

        // GET: api/Variations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Variation>> GetVariation(int id)
        {
            var variation = await _context.Variations.FindAsync(id);

            if (variation == null)
            {
                return NotFound();
            }

            return variation;
        }

        // PUT: api/Variations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariation(int id, Variation variation)
        {
            if (id != variation.VariationId)
            {
                return BadRequest();
            }

            _context.Entry(variation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariationExists(id))
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

        // POST: api/Variations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Variation>> PostVariation(Variation variation)
        {
            _context.Variations.Add(variation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVariation", new { id = variation.VariationId }, variation);
        }

        // DELETE: api/Variations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariation(int id)
        {
            var variation = await _context.Variations.FindAsync(id);
            if (variation == null)
            {
                return NotFound();
            }

            _context.Variations.Remove(variation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VariationExists(int id)
        {
            return _context.Variations.Any(e => e.VariationId == id);
        }
    }
}
