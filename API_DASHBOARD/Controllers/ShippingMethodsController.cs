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
    public class ShippingMethodsController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public ShippingMethodsController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/ShippingMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingMethod>>> GetShippingMethods()
        {
            return await _context.ShippingMethods.ToListAsync();
        }

        // GET: api/ShippingMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethod(int id)
        {
            var shippingMethod = await _context.ShippingMethods.FindAsync(id);

            if (shippingMethod == null)
            {
                return NotFound();
            }

            return shippingMethod;
        }

        // PUT: api/ShippingMethods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingMethod(int id, ShippingMethod shippingMethod)
        {
            if (id != shippingMethod.ShippingMethodId)
            {
                return BadRequest();
            }

            _context.Entry(shippingMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingMethodExists(id))
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

        // POST: api/ShippingMethods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShippingMethod>> PostShippingMethod(ShippingMethod shippingMethod)
        {
            _context.ShippingMethods.Add(shippingMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingMethod", new { id = shippingMethod.ShippingMethodId }, shippingMethod);
        }

        // DELETE: api/ShippingMethods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingMethod(int id)
        {
            var shippingMethod = await _context.ShippingMethods.FindAsync(id);
            if (shippingMethod == null)
            {
                return NotFound();
            }

            _context.ShippingMethods.Remove(shippingMethod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShippingMethodExists(int id)
        {
            return _context.ShippingMethods.Any(e => e.ShippingMethodId == id);
        }
    }
}
