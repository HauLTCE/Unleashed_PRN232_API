using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.Services.IServices;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        // Hỗ trợ phân trang & tìm kiếm qua PaginationParams (Search, PageNumber, PageSize)
        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryDetailDTO>>> GetCategories([FromQuery] PaginationParams pagination)
        {
            var result = await _categoryService.GetPagedCategoriesAsync(pagination);
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDetailDTO>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] UpdateCategoryDTO updateCategoryDto)
        {
            try
            {
                var updated = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the category");
            }
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<CategoryDetailDTO>> PostCategory([FromBody] CreateCategoryDTO createCategoryDto)
        {
            try
            {
                var created = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtRoute("GetCategory", new { id = created.CategoryId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the category");
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var ok = await _categoryService.DeleteCategoryAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
