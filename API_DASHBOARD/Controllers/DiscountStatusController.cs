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
    public class DiscountStatusController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public DiscountStatusController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/DiscountStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountStatus>>> GetDiscountStatuses()
        {
            return await _context.DiscountStatuses.ToListAsync();
        }

        // GET: api/DiscountStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountStatus>> GetDiscountStatus(int id)
        {
            var discountStatus = await _context.DiscountStatuses.FindAsync(id);

            if (discountStatus == null)
            {
                return NotFound();
            }

            return discountStatus;
        }

        // PUT: api/DiscountStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscountStatus(int id, DiscountStatus discountStatus)
        {
            if (id != discountStatus.DiscountStatusId)
            {
                return BadRequest();
            }

            _context.Entry(discountStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountStatusExists(id))
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

        // POST: api/DiscountStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DiscountStatus>> PostDiscountStatus(DiscountStatus discountStatus)
        {
            _context.DiscountStatuses.Add(discountStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiscountStatus", new { id = discountStatus.DiscountStatusId }, discountStatus);
        }

        // DELETE: api/DiscountStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountStatus(int id)
        {
            var discountStatus = await _context.DiscountStatuses.FindAsync(id);
            if (discountStatus == null)
            {
                return NotFound();
            }

            _context.DiscountStatuses.Remove(discountStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscountStatusExists(int id)
        {
            return _context.DiscountStatuses.Any(e => e.DiscountStatusId == id);
        }
    }
}
