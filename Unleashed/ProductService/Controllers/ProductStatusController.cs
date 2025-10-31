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
    public class ProductStatusController : ControllerBase
    {
        private readonly IProductStatusService _productStatusService;

        public ProductStatusController(IProductStatusService productStatusService)
        {
            _productStatusService = productStatusService;
        }

        /// <summary>
        /// Lấy toàn bộ trạng thái sản phẩm
        /// (ví dụ: Active, Inactive, Draft, OutOfStock...).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductStatus>>> GetAllAsync()
        {
            var statuses = await _productStatusService.GetAllAsync();
            return Ok(statuses);
        }

        /// <summary>
        /// Lấy thông tin trạng thái theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductStatus>> GetByIdAsync([FromRoute] int id)
        {
            var status = await _productStatusService.GetByIdAsync(id);

            if (status == null)
                return NotFound($"ProductStatus with id = {id} not found.");

            return Ok(status);
        }
    }
}
