using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.BrandDTOs;
using ProductService.DTOs.Common;
using ProductService.Services.IServices;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // GET: api/Brands
        // Hỗ trợ phân trang + tìm kiếm qua PaginationParams (Search, PageNumber, PageSize)
        [HttpGet]
        public async Task<ActionResult<PagedResult<BrandDetailDTO>>> GetBrands([FromQuery] PaginationParams pagination)
        {
            var result = await _brandService.GetPagedBrandsAsync(pagination);
            return Ok(result);
        }

        // GET: api/Brands/5
        [HttpGet("{id:int}", Name = "GetBrand")]
        public async Task<ActionResult<BrandDetailDTO>> GetBrand(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }
        // PUT: api/Brands/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> PutBrand(int id, [FromBody] UpdateBrandDTO updateBrandDto)
        {
            try
            {
                var updated = await _brandService.UpdateBrandAsync(id, updateBrandDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the brand");
            }
        }
        // POST: api/Brands
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<BrandDetailDTO>> PostBrand([FromBody] CreateBrandDTO createBrandDto)
        {
            try
            {
                var created = await _brandService.CreateBrandAsync(createBrandDto);
                return CreatedAtRoute("GetBrand", new { id = created.BrandId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the brand");
            }
        }
        // DELETE: api/Brands/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var ok = await _brandService.DeleteBrandAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
