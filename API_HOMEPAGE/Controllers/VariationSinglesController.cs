using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_HOMEPAGE.Models;

namespace API_HOMEPAGE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariationSinglesController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public VariationSinglesController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/VariationSingles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VariationSingle>>> GetVariationSingles()
        {
            return await _context.VariationSingles.ToListAsync();
        }

        // GET: api/VariationSingles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VariationSingle>> GetVariationSingle(int id)
        {
            var variationSingle = await _context.VariationSingles.FindAsync(id);

            if (variationSingle == null)
            {
                return NotFound();
            }

            return variationSingle;
        }

        // PUT: api/VariationSingles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariationSingle(int id, VariationSingle variationSingle)
        {
            if (id != variationSingle.VariationSingleId)
            {
                return BadRequest();
            }

            _context.Entry(variationSingle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariationSingleExists(id))
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

        // POST: api/VariationSingles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VariationSingle>> PostVariationSingle(VariationSingle variationSingle)
        {
            _context.VariationSingles.Add(variationSingle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVariationSingle", new { id = variationSingle.VariationSingleId }, variationSingle);
        }

        // DELETE: api/VariationSingles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariationSingle(int id)
        {
            var variationSingle = await _context.VariationSingles.FindAsync(id);
            if (variationSingle == null)
            {
                return NotFound();
            }

            _context.VariationSingles.Remove(variationSingle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VariationSingleExists(int id)
        {
            return _context.VariationSingles.Any(e => e.VariationSingleId == id);
        }
    }
}
