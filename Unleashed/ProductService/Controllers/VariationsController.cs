// Path: ProductService/Controllers/VariationsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.VariationDTOs;
using ProductService.Models;
using ProductService.Services.IServices;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariationsController : ControllerBase
    {
        private readonly IVariationQueryService _service;
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly ILogger<VariationsController> _logger;

        public VariationsController(IVariationQueryService service, IColorService colorService,
            ISizeService sizeService, ILogger<VariationsController> logger)
        {
            _service = service;
            _colorService = colorService;
            _sizeService = sizeService;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/variations/{id}
        /// 200: trả về VariationDetailDTO
        /// 404: không có
        /// 500: lỗi nội bộ
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VariationDetailDTO>> GetById([FromRoute] int id)
        {
            try
            {
                var dto = await _service.GetDetailByIdAsync(id);
                if (dto == null) return NotFound();
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET Variation failed. Id={Id}", id);
                return StatusCode(500, "Internal error");
            }
        }

        /// <summary>
        /// POST /api/variations/batch
        /// Body: [101,202,303,404]
        /// 200: trả “found-only”; nếu không có gì -> []
        /// </summary>
        [HttpPost("batch")]
        public async Task<ActionResult<List<VariationDetailDTO>>> GetBatch([FromBody] List<int> ids)
        {
            try
            {
                var result = await _service.GetDetailsByIdsAsync(ids ?? new List<int>());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST batch variations failed");
                return StatusCode(500, "Internal error");
            }
        }

        /// <summary>
        /// GET /api/variations?search=&productId=&colorId=&sizeId=
        /// Search theo: ProductName/ProductCode, ColorName, SizeName + filter theo Ids
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<VariationDetailDTO>>> Search(
            [FromQuery] string? search,
            [FromQuery] Guid? productId,
            [FromQuery] int? colorId,
            [FromQuery] int? sizeId)
        {
            try
            {
                var result = await _service.SearchDetailsAsync(search, productId, colorId, sizeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search variations failed");
                return StatusCode(500, "Internal error");
            }
        }
        // ========== COLOR SECTION ==========

        /// <summary>
        /// GET /api/variations/colors
        /// </summary>
        [HttpGet("colors")]
        public async Task<ActionResult<List<Color>>> GetColors()
        {
            try
            {
                var colors = await _colorService.GetAllAsync();
                return Ok(colors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET colors failed");
                return StatusCode(500, "Internal error");
            }
        }

        /// <summary>
        /// GET /api/variations/colors/available
        /// </summary>
        [HttpGet("colors/available")]
        public async Task<ActionResult<List<Color>>> GetAvailableColors([FromQuery] bool onlyActive = false)
        {
            try
            {
                var colors = await _colorService.GetAvailableAsync(onlyActive);
                return Ok(colors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET available colors failed");
                return StatusCode(500, "Internal error");
            }
        }

        // ========== SIZE SECTION ==========

        /// <summary>
        /// GET /api/variations/sizes
        /// </summary>
        [HttpGet("sizes")]
        public async Task<ActionResult<List<Size>>> GetSizes()
        {
            try
            {
                var sizes = await _sizeService.GetAllAsync();
                return Ok(sizes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET sizes failed");
                return StatusCode(500, "Internal error");
            }
        }

        /// <summary>
        /// GET /api/variations/sizes/available
        /// </summary>
        [HttpGet("sizes/available")]
        public async Task<ActionResult<List<Size>>> GetAvailableSizes([FromQuery] bool onlyActive = false)
        {
            try
            {
                var sizes = await _sizeService.GetAvailableAsync(onlyActive);
                return Ok(sizes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET available sizes failed");
                return StatusCode(500, "Internal error");
            }
        }
    }
}
