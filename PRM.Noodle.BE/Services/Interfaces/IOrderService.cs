using System;
using Services.DTOs.Order;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{ public interface IOrderService
    {

        Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<PagedOrderResponse> GetOrdersAsync(OrderQueryDto queryDto);
        Task<PagedOrderResponse> GetOrdersByUserIdAsync(int userId, int page = 1, int pageSize = 10);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<OrderDto?> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> ConfirmOrderAsync(int orderId);
        Task<bool> CompleteOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);

        Task<PagedOrderResponse> GetTodayOrdersAsync(int page = 1, int pageSize = 10);
        Task<PagedOrderResponse> GetPendingOrdersAsync(int page = 1, int pageSize = 10);
        Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);
        OrderStatusesDto GetOrderStatuses();
    }
  
}
