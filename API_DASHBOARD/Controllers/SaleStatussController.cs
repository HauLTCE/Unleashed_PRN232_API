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
    public class SaleStatussController : ControllerBase
    {
        private readonly UnleashedContext _context;

        public SaleStatussController(UnleashedContext context)
        {
            _context = context;
        }

        // GET: api/SaleStatuss
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleStatus>>> GetSaleStatuses()
        {
            return await _context.SaleStatuses.ToListAsync();
        }

        // GET: api/SaleStatuss/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleStatus>> GetSaleStatus(int id)
        {
            var saleStatus = await _context.SaleStatuses.FindAsync(id);

            if (saleStatus == null)
            {
                return NotFound();
            }

            return saleStatus;
        }

        // PUT: api/SaleStatuss/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleStatus(int id, SaleStatus saleStatus)
        {
            if (id != saleStatus.SaleStatusId)
            {
                return BadRequest();
            }

            _context.Entry(saleStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleStatusExists(id))
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

        // POST: api/SaleStatuss
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SaleStatus>> PostSaleStatus(SaleStatus saleStatus)
        {
            _context.SaleStatuses.Add(saleStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSaleStatus", new { id = saleStatus.SaleStatusId }, saleStatus);
        }

        // DELETE: api/SaleStatuss/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleStatus(int id)
        {
            var saleStatus = await _context.SaleStatuses.FindAsync(id);
            if (saleStatus == null)
            {
                return NotFound();
            }

            _context.SaleStatuses.Remove(saleStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleStatusExists(int id)
        {
            return _context.SaleStatuses.Any(e => e.SaleStatusId == id);
        }
    }
}
