// Path: ProductService/Controllers/VariationsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.VariationDTOs;
using ProductService.Services.IServices;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariationsController : ControllerBase
    {
        private readonly IVariationQueryService _service;
        private readonly ILogger<VariationsController> _logger;

        public VariationsController(IVariationQueryService service, ILogger<VariationsController> logger)
        {
            _service = service;
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
    }
}
