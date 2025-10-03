using InventoryService.DTOs.Provider;
using InventoryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProvidersController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        // GET: api/Providers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProviderDto>>> GetProviders()
        {
            var providers = await _providerService.GetAllProvidersAsync();
            return Ok(providers);
        }

        // GET: api/Providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProviderDto>> GetProvider(int id)
        {
            var provider = await _providerService.GetProviderByIdAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            return Ok(provider);
        }

        // PUT: api/Providers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvider(int id, UpdateProviderDto providerDto)
        {
            var updateResult = await _providerService.UpdateProviderAsync(id, providerDto);

            if (!updateResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Providers
        [HttpPost]
        public async Task<ActionResult<ProviderDto>> PostProvider(CreateProviderDto providerDto)
        {
            var createdProvider = await _providerService.CreateProviderAsync(providerDto);
            return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.ProviderId }, createdProvider);
        }

        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(int id)
        {
            var deleteResult = await _providerService.DeleteProviderAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}