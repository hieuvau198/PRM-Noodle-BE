using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Order
{
    // DTO cho việc tạo đơn hàng mới
    public class CreateOrderDto
    {

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [MaxLength(500, ErrorMessage = "Delivery address cannot exceed 500 characters")]
        public string? DeliveryAddress { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(cash|card|digital_wallet)$", ErrorMessage = "Payment method must be cash, card, or digital_wallet")]
        public string PaymentMethod { get; set; } = "cash";

        public List<CreateOrderItemDto>? OrderItems { get; set; } = new List<CreateOrderItemDto>();

        public List<CreateOrderComboDto>? OrderCombos { get; set; } = new List<CreateOrderComboDto>();

        public bool HasItems => (OrderItems?.Count > 0) || (OrderCombos?.Count > 0);
    }
    // DTO cho OrderItem trong đơn hàng mới
    public class CreateOrderItemDto
    {
       
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Danh sách topping cho sản phẩm này
        public List<CreateOrderItemToppingDto>? Toppings { get; set; } = new List<CreateOrderItemToppingDto>();
    }

    // DTO cho OrderCombo trong đơn hàng mới
    public class CreateOrderComboDto
    {
        
        public int ComboId { get; set; }

       
        public int Quantity { get; set; }
    }

    // DTO cho Topping trong OrderItem
    public class CreateOrderItemToppingDto
    {
      
        public int ToppingId { get; set; }

        public int Quantity { get; set; }
    }

    // DTO cho cập nhật đơn hàng
    public class UpdateOrderDto
    {
        [RegularExpression("^(pending|confirmed|preparing|ready|delivered|cancelled)$",
            ErrorMessage = "Invalid order status")]
        public string? OrderStatus { get; set; }

        [RegularExpression("^(pending|paid|failed)$",
            ErrorMessage = "Invalid payment status")]
        public string? PaymentStatus { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? Notes { get; set; }
    }

    // DTO trả về thông tin đơn hàng
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Notes { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public List<OrderComboDto> OrderCombos { get; set; } = new List<OrderComboDto>();
    }

    // DTO cho OrderItem trong response
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        public List<OrderItemToppingDto> Toppings { get; set; } = new List<OrderItemToppingDto>();
    }

    // DTO cho OrderCombo trong response
    public class OrderComboDto
    {
        public int OrderComboId { get; set; }
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    // DTO cho OrderItemTopping trong response
    public class OrderItemToppingDto
    {
        public int OrderItemToppingId { get; set; }
        public int ToppingId { get; set; }
        public string? ToppingName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    // DTO cho danh sách đơn hàng với phân trang
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public int TotalItems { get; set; } 
    }

    // DTO cho query parameters
    public class OrderQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? UserId { get; set; }
    }

    // DTO cho response có phân trang
    public class PagedOrderResponse
    {
        public List<OrderListDto> Orders { get; set; } = new List<OrderListDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    // DTO cho báo cáo doanh thu
    public class RevenueReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailyRevenueDto> DailyRevenueBreakdown { get; set; } = new List<DailyRevenueDto>();
        public List<OrderStatusSummaryDto> OrderStatusSummary { get; set; } = new List<OrderStatusSummaryDto>();
    }

    // DTO cho doanh thu hàng ngày
    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    // DTO cho tóm tắt trạng thái đơn hàng
    public class OrderStatusSummaryDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // DTO cho danh sách trạng thái đơn hàng
    public class OrderStatusesDto
    {
        public List<string> OrderStatuses { get; set; } = new List<string>();
        public List<string> PaymentStatuses { get; set; } = new List<string>();
        public List<string> PaymentMethods { get; set; } = new List<string>();
    }
}