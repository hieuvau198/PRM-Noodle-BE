using Microsoft.AspNetCore.Mvc;
using PRM.Noodle.BE.Service.Orders.Models;
using PRM.Noodle.BE.Service.Orders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.CreateOrderAsync(createOrderDto);
                if (result == null)
                    return BadRequest(new { message = "Failed to create order." });

                return CreatedAtAction(nameof(GetOrderById), new { orderId = result.OrderId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order.", details = ex.Message });
            }
        }


        [HttpGet]

        public async Task<ActionResult<PagedOrderResponse>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? orderStatus = null,
            [FromQuery] string? paymentStatus = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var queryDto = new OrderQueryDto
                {
                    Page = page,
                    PageSize = pageSize,
                    OrderStatus = orderStatus,
                    PaymentStatus = paymentStatus,
                    UserId = userId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var result = await _orderService.GetOrdersAsync(queryDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders.", details = ex.Message });
            }
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PagedOrderResponse>> GetOrdersByUserId(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Invalid user ID." });

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _orderService.GetOrdersByUserIdAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user orders.", details = ex.Message });
            }
        }


        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.GetOrderByIdAsync(orderId);
                if (result == null)
                    return NotFound(new { message = "Order not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order.", details = ex.Message });
            }
        }


        [HttpPut("{orderId}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int orderId, [FromBody] UpdateOrderDto updateOrderDto)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.UpdateOrderAsync(orderId, updateOrderDto);
                if (result == null)
                    return NotFound(new { message = "Order not found." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order.", details = ex.Message });
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.DeleteOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order not found ." });

                return Ok(new { message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Xác nhận đơn hàng (chuyển từ pending sang confirmed)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <returns>Kết quả xác nhận</returns>
        [HttpPatch("{orderId}/confirm")]
        public async Task<ActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.ConfirmOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order cannot be confirmed. Order not found or invalid status." });

                return Ok(new { message = "Order confirmed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while confirming the order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Hoàn thành đơn hàng (chuyển sang delivered)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <returns>Kết quả hoàn thành</returns>
        [HttpPatch("{orderId}/complete")]
        public async Task<ActionResult> CompleteOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.CompleteOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order cannot be completed. Order not found or invalid status." });

                return Ok(new { message = "Order completed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while completing the order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái chuẩn bị (từ confirmed sang preparing)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <returns>Kết quả chuyển trạng thái</returns>
        [HttpPatch("{orderId}/prepare")]
        public async Task<ActionResult> PrepareOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.PrepareOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order cannot be prepared. Order not found or invalid status (must be confirmed)." });

                return Ok(new { message = "Order preparation started successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while preparing the order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái sẵn sàng giao/vận chuyển (từ preparing sang ready)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <returns>Kết quả chuyển trạng thái</returns>
        [HttpPatch("{orderId}/deliver")]
        public async Task<ActionResult> DeliverOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.DeliverOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order cannot be set for delivery. Order not found or invalid status (must be preparing)." }); // ✅ SỬA

                return Ok(new { message = "Order is ready for delivery." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while setting order for delivery.", details = ex.Message });
            }
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <returns>Kết quả hủy</returns>
        [HttpPatch("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                var result = await _orderService.CancelOrderAsync(orderId);
                if (!result)
                    return BadRequest(new { message = "Order cannot be cancelled. Order not found or invalid status." });

                return Ok(new { message = "Order cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cancelling the order.", details = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<PagedOrderResponse>> GetTodayOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _orderService.GetTodayOrdersAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving today's orders.", details = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<PagedOrderResponse>> GetPendingOrders(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _orderService.GetPendingOrdersAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending orders.", details = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                // Nếu không có tham số thì lấy doanh thu hôm nay
                var today = DateTime.Today;
                fromDate ??= today;
                toDate ??= today.AddDays(1).AddTicks(-1); // Cuối ngày hôm nay

                var result = await _orderService.GetRevenueReportAsync(fromDate.Value, toDate.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving revenue report.", details = ex.Message });
            }
        }
        [HttpGet("statuses")]
        public ActionResult<OrderStatusesDto> GetOrderStatuses()
        {
            try
            {
                var result = _orderService.GetOrderStatuses();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving order statuses.", details = ex.Message });
            }
        }
        [HttpPost("{orderId}/items")]
        public async Task<ActionResult<OrderItemDto>> AddItemToOrder(int orderId, [FromBody] AddOrderItemDto addItemDto)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.AddItemToOrderAsync(orderId, addItemDto);
                if (result == null)
                    return BadRequest(new { message = "Failed to add item to order. Order not found or cannot be modified." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding item to order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Xóa món khỏi đơn hàng
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="itemId">ID của OrderItem cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{orderId}/items/{itemId}")]
        public async Task<ActionResult> RemoveItemFromOrder(int orderId, int itemId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (itemId <= 0)
                    return BadRequest(new { message = "Invalid item ID." });

                var result = await _orderService.RemoveItemFromOrderAsync(orderId, itemId);
                if (!result)
                    return BadRequest(new { message = "Failed to remove item from order. Order or item not found, or order cannot be modified." });

                return Ok(new { message = "Item removed from order successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing item from order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Chỉnh sửa món trong đơn hàng (số lượng, topping)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="itemId">ID của OrderItem cần chỉnh sửa</param>
        /// <param name="updateItemDto">Thông tin cập nhật</param>
        /// <returns>Thông tin OrderItem đã được cập nhật</returns>
        [HttpPatch("{orderId}/items/{itemId}")]
        public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(int orderId, int itemId, [FromBody] UpdateOrderItemDto updateItemDto)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (itemId <= 0)
                    return BadRequest(new { message = "Invalid item ID." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.UpdateOrderItemAsync(orderId, itemId, updateItemDto);
                if (result == null)
                    return BadRequest(new { message = "Failed to update order item. Order or item not found, or order cannot be modified." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating order item.", details = ex.Message });
            }
        }

        // ===== OrderCombo Management =====

        /// <summary>
        /// Thêm combo vào đơn hàng
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="addComboDto">Thông tin combo cần thêm</param>
        /// <returns>Thông tin OrderCombo đã được thêm</returns>
        [HttpPost("{orderId}/combos")]
        public async Task<ActionResult<OrderComboDto>> AddComboToOrder(int orderId, [FromBody] AddOrderComboDto addComboDto)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _orderService.AddComboToOrderAsync(orderId, addComboDto);
                if (result == null)
                    return BadRequest(new { message = "Failed to add combo to order. Order not found or cannot be modified." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding combo to order.", details = ex.Message });
            }
        }

        /// <summary>
        /// Xóa combo khỏi đơn hàng
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="comboId">ID của OrderCombo cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{orderId}/combos/{comboId}")]
        public async Task<ActionResult> RemoveComboFromOrder(int orderId, int comboId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest(new { message = "Invalid order ID." });

                if (comboId <= 0)
                    return BadRequest(new { message = "Invalid combo ID." });

                var result = await _orderService.RemoveComboFromOrderAsync(orderId, comboId);
                if (!result)
                    return BadRequest(new { message = "Failed to remove combo from order. Order or combo not found, or order cannot be modified." });

                return Ok(new { message = "Combo removed from order successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing combo from order.", details = ex.Message });
            }
        }
    }
}
