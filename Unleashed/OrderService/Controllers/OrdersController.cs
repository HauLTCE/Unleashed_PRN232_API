﻿using Microsoft.AspNetCore.Authorization; // Thêm thư viện Authorization
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.DTOs;
using OrderService.Services.Interfaces;
using System.Security.Claims; // Thêm thư viện Claims

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Bật Authorize cho toàn bộ controller
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/order - Endpoint được nâng cấp
        [HttpGet]
        // [Authorize(Roles = "STAFF,ADMIN")] // Phân quyền cho API
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] int? statusId,
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            var result = await _orderService.GetAllOrdersAsync(search, sort, statusId, page, size);
            return Ok(result);
        }

        // GET: api/order/{orderId}
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Message = $"Đơn hàng với ID {orderId} không tồn tại." });
            }
            return Ok(order);
        }

        // GET: api/order/my-orders (Lấy đơn hàng của người dùng đang đăng nhập)
        [HttpGet("my-orders")]
        // [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyOrders()
        {
            // Lấy UserId từ token/context
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(userId);
            return Ok(orders);
        }

        // POST: api/order/check-stock
        [HttpPost("check-stock")]
        // [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckStockAvailability([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                await _orderService.CheckStockAvailabilityAsync(createOrderDto);
                return Ok(new { Message = "Sản phẩm có sẵn." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/order
        [HttpPost]
        // [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            //// Gán UserId của người dùng đang đăng nhập
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            //{
            //    return Unauthorized();
            //}
            createOrderDto.UserId = Guid.Parse("910FF8D2-05BE-4F7B-9E8C-B053FBF6F1F6");

            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PUT: api/order/{orderId}/cancel
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderId);
                return Ok(new { Message = "Hủy đơn hàng thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/order/{orderId}/staff-review
        [HttpPut("{orderId}/staff-review")]
        // [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> ReviewOrder(Guid orderId, [FromBody] ReviewOrderDto reviewDto)
        {
            var staffIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffIdString) || !Guid.TryParse(staffIdString, out Guid staffId))
            {
                return Unauthorized();
            }

            try
            {
                await _orderService.ReviewOrderByStaffAsync(orderId, staffId, reviewDto.IsApproved);
                return Ok(new { Message = "Đã duyệt đơn hàng." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

       
    }
}