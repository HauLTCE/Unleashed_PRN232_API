using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.OtherDTOs;
using ProductService.Services.IServices;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly IFilterService _filterService;

        public FiltersController(IFilterService filterService)
        {
            _filterService = filterService;
        }

        /// <summary>
        /// Lấy options filter color/size.
        /// onlyAvailable = true => chỉ trả các giá trị đang có trong variations.
        /// onlyActiveProducts = true => lọc theo sản phẩm Active (kết hợp với onlyAvailable).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<FilterOptionsDTO>> GetFilterOptions(
            [FromQuery] bool onlyAvailable = false,
            [FromQuery] bool onlyActiveProducts = false)
        {
            var options = await _filterService.GetFilterOptionsAsync(onlyAvailable, onlyActiveProducts);
            return Ok(options);
        }
    }
}
