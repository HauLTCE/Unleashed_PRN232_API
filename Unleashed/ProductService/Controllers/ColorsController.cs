using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách màu (kể cả màu không còn dùng).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Color>>> GetAllAsync()
        {
            var colors = await _colorService.GetAllAsync();
            return Ok(colors);
        }

        /// <summary>
        /// Lấy danh sách màu còn "available".
        /// onlyActiveProducts = true => chỉ trả về màu đang được dùng trong các sản phẩm active.
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<List<Color>>> GetAvailableAsync([FromQuery] bool onlyActiveProducts = false)
        {
            var colors = await _colorService.GetAvailableAsync(onlyActiveProducts);
            return Ok(colors);
        }

        /// <summary>
        /// Lấy thông tin màu theo Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Color>> GetByIdAsync([FromRoute] int id)
        {
            var color = await _colorService.GetByIdAsync(id);

            if (color == null)
                return NotFound($"Color with id = {id} not found.");

            return Ok(color);
        }
    }
}
