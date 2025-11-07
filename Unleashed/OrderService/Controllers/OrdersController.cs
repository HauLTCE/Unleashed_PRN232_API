using Microsoft.AspNetCore.Authorization; // Thêm thư viện Authorization
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
        public async Task<IActionResult> GetMyOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var pagedResult = await _orderService.GetOrdersByCustomerIdAsync(userId, pageNumber, pageSize);

            return Ok(pagedResult);
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Gán UserId của người dùng đang đăng nhập
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            //{
            //    return Unauthorized();
            //}
            //createOrderDto.UserId = Guid.Parse(userIdString);
            createOrderDto.UserId = createOrderDto.UserId;
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
                await _orderService.ReviewOrderByStaffAsync(orderId, staffId, reviewDto.orderStatus);
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

        [HttpPut("{orderId}/confirm-receipt")]
        public async Task<IActionResult> ConfirmOrderReceived(Guid orderId)
        {
            // Kiểm tra quyền sở hữu
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId.ToString() != userIdString) return Forbid();

            try
            {
                await _orderService.ConfirmOrderReceivedAsync(orderId);
                return Ok(new { Message = "Xác nhận đã nhận hàng thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPut("{orderId}/ship")]
        public async Task<IActionResult> ShipOrder(Guid orderId)
        {
            var staffIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var staffId = Guid.Parse(staffIdString!);

            try
            {
                // Phương thức này xử lý việc chuyển từ Processing -> Shipping
                await _orderService.UpdateOrderStatusByStaffAsync(orderId, staffId, 3); // 3 là Shipping
                return Ok(new { Message = "Đơn hàng đã được chuyển sang trạng thái đang giao." });
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

        [HttpGet("user/{userId}/eligible-for-review")]
        public async Task<IActionResult> GetEligibleOrdersForReview(Guid userId, [FromQuery] Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return BadRequest("A valid Product ID is required.");
            }

            var result = await _orderService.GetEligibleOrdersForReviewAsync(userId, productId);
            return Ok(result);
        }

    }
}