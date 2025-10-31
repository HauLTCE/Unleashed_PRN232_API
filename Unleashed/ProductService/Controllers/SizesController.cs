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
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        /// <summary>
        /// Lấy toàn bộ size (bao gồm cả size không còn dùng).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Size>>> GetAllAsync()
        {
            var sizes = await _sizeService.GetAllAsync();
            return Ok(sizes);
        }

        /// <summary>
        /// Lấy các size còn đang được dùng trong sản phẩm.
        /// onlyActiveProducts = true => chỉ size thuộc sản phẩm active.
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<List<Size>>> GetAvailableAsync([FromQuery] bool onlyActiveProducts = false)
        {
            var sizes = await _sizeService.GetAvailableAsync(onlyActiveProducts);
            return Ok(sizes);
        }

        /// <summary>
        /// Lấy thông tin size theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Size>> GetByIdAsync([FromRoute] int id)
        {
            var size = await _sizeService.GetByIdAsync(id);

            if (size == null)
                return NotFound($"Size with id = {id} not found.");

            return Ok(size);
        }
    }
    }
