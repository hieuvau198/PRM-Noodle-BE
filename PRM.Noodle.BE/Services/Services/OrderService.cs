using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories.Persistence;
using Services.DTOs.Order;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Validate order has at least one item or combo
            if ((createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any()) &&
                (createOrderDto.OrderCombos == null || !createOrderDto.OrderCombos.Any()))
            {
                throw new ArgumentException("Order must contain at least one product or combo");
            }

            // Validate user exists
            var userExists = await _unitOfWork.Users.ExistsAsync(u => u.UserId == createOrderDto.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"User with ID {createOrderDto.UserId} does not exist");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create order entity
                var order = _mapper.Map<Order>(createOrderDto);
                order.OrderDate = DateTime.Now;
                order.OrderStatus = "pending";
                order.PaymentStatus = "pending";

                // Add order
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync(); // Save to get OrderId

                decimal totalAmount = 0;

                // Process order items (products)
                if (createOrderDto.OrderItems != null && createOrderDto.OrderItems.Any())
                {
                    foreach (var itemDto in createOrderDto.OrderItems)
                    {
                        // Validate product exists
                        var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                        if (product == null)
                        {
                            throw new ArgumentException($"Product with ID {itemDto.ProductId} does not exist");
                        }

                        if (itemDto.Quantity <= 0)
                        {
                            throw new ArgumentException($"Product quantity must be greater than 0");
                        }

                        var orderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = product.BasePrice,
                            Subtotal = product.BasePrice * itemDto.Quantity
                        };

                        await _unitOfWork.OrderItems.AddAsync(orderItem);
                        await _unitOfWork.CompleteAsync(); // Save to get OrderItemId

                        totalAmount += orderItem.Subtotal;

                        // Process toppings for this item
                        if (itemDto.Toppings != null && itemDto.Toppings.Any())
                        {
                            foreach (var toppingDto in itemDto.Toppings)
                            {
                                // Validate topping exists
                                var topping = await _unitOfWork.Toppings.GetByIdAsync(toppingDto.ToppingId);
                                if (topping == null)
                                {
                                    throw new ArgumentException($"Topping with ID {toppingDto.ToppingId} does not exist");
                                }

                                if (toppingDto.Quantity <= 0)
                                {
                                    throw new ArgumentException($"Topping quantity must be greater than 0");
                                }

                                var orderItemTopping = new OrderItemTopping
                                {
                                    OrderItemId = orderItem.OrderItemId,
                                    ToppingId = toppingDto.ToppingId,
                                    Quantity = toppingDto.Quantity,
                                    UnitPrice = topping.Price,
                                    Subtotal = topping.Price * toppingDto.Quantity
                                };

                                await _unitOfWork.OrderItemTopping.AddAsync(orderItemTopping);
                                totalAmount += orderItemTopping.Subtotal;
                            }
                        }
                    }
                }

                // Process order combos
                if (createOrderDto.OrderCombos != null && createOrderDto.OrderCombos.Any())
                {
                    foreach (var comboDto in createOrderDto.OrderCombos)
                    {
                        // Validate combo exists
                        var combo = await _unitOfWork.Combos.GetByIdAsync(comboDto.ComboId);
                        if (combo == null)
                        {
                            throw new ArgumentException($"Combo with ID {comboDto.ComboId} does not exist");
                        }

                        if (comboDto.Quantity <= 0)
                        {
                            throw new ArgumentException($"Combo quantity must be greater than 0");
                        }

                        var orderCombo = new OrderCombo
                        {
                            OrderId = order.OrderId,
                            ComboId = comboDto.ComboId,
                            Quantity = comboDto.Quantity,
                            UnitPrice = combo.ComboPrice,
                            Subtotal = combo.ComboPrice * comboDto.Quantity
                        };

                        await _unitOfWork.OrderCombos.AddAsync(orderCombo);
                        totalAmount += orderCombo.Subtotal;
                    }
                }

                // Update total amount
                order.TotalAmount = totalAmount;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return the created order with full details
                return await GetOrderByIdAsync(order.OrderId);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PagedOrderResponse> GetOrdersAsync(OrderQueryDto queryDto)
        {
            var query = _unitOfWork.Orders.GetQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderCombos)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(queryDto.OrderStatus))
                query = query.Where(o => o.OrderStatus == queryDto.OrderStatus);

            if (!string.IsNullOrEmpty(queryDto.PaymentStatus))
                query = query.Where(o => o.PaymentStatus == queryDto.PaymentStatus);

            if (queryDto.UserId.HasValue)
                query = query.Where(o => o.UserId == queryDto.UserId.Value);

            if (queryDto.FromDate.HasValue)
                query = query.Where(o => o.OrderDate >= queryDto.FromDate.Value);

            if (queryDto.ToDate.HasValue)
                query = query.Where(o => o.OrderDate <= queryDto.ToDate.Value);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize)
                .ToListAsync();

            var orderListDtos = _mapper.Map<List<OrderListDto>>(orders);

            return new PagedOrderResponse
            {
                Orders = orderListDtos,
                TotalCount = totalCount,
                Page = queryDto.Page,
                PageSize = queryDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            };
        }

        public async Task<PagedOrderResponse> GetOrdersByUserIdAsync(int userId, int page = 1, int pageSize = 10)
        {
            var queryDto = new OrderQueryDto
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };

            return await GetOrdersAsync(queryDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemToppings)
                        .ThenInclude(oit => oit.Topping)
                .Include(o => o.OrderCombos)
                    .ThenInclude(oc => oc.Combo)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> UpdateOrderAsync(int orderId, UpdateOrderDto updateOrderDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return null;

            // Cập nhật các field được cung cấp
            if (!string.IsNullOrEmpty(updateOrderDto.OrderStatus))
            {
                order.OrderStatus = updateOrderDto.OrderStatus;

                // Set timestamps dựa trên trạng thái
                if (updateOrderDto.OrderStatus == "confirmed" && order.ConfirmedAt == null)
                    order.ConfirmedAt = DateTime.Now;
                else if (updateOrderDto.OrderStatus == "delivered" && order.CompletedAt == null)
                    order.CompletedAt = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(updateOrderDto.PaymentStatus))
                order.PaymentStatus = updateOrderDto.PaymentStatus;

            if (!string.IsNullOrEmpty(updateOrderDto.DeliveryAddress))
                order.DeliveryAddress = updateOrderDto.DeliveryAddress;

            if (!string.IsNullOrEmpty(updateOrderDto.Notes))
                order.Notes = updateOrderDto.Notes;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return false;

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> ConfirmOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return false;

            order.OrderStatus = "confirmed";
            order.ConfirmedAt = DateTime.Now;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            // KHÔNG cho phép complete nếu đã completed hoặc đã cancelled
            if (order == null || order.OrderStatus == "completed" || order.OrderStatus == "cancelled")
                return false;

            order.OrderStatus = "completed";
            order.CompletedAt = DateTime.Now;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null || order.OrderStatus != "pending")
                return false;

            order.OrderStatus = "cancelled";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<PagedOrderResponse> GetTodayOrdersAsync(int page = 1, int pageSize = 10)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var queryDto = new OrderQueryDto
            {
                Page = page,
                PageSize = pageSize,
                FromDate = today,
                ToDate = tomorrow.AddTicks(-1) // End of today
            };

            return await GetOrdersAsync(queryDto);
        }

        public async Task<PagedOrderResponse> GetPendingOrdersAsync(int page = 1, int pageSize = 10)
        {
            var queryDto = new OrderQueryDto
            {
                Page = page,
                PageSize = pageSize,
                OrderStatus = "pending"
            };

            return await GetOrdersAsync(queryDto);
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate)
        {
            var orders = await _unitOfWork.Orders.GetQueryable()
                .Where(o => o.OrderDate.HasValue && o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .ToListAsync();

            var completedOrders = orders.Where(o => o.OrderStatus == "completed").ToList();
            var cancelledOrders = orders.Where(o => o.OrderStatus == "cancelled").ToList();

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var completedOrdersCount = completedOrders.Count;
            var cancelledOrdersCount = cancelledOrders.Count;
            var averageOrderValue = completedOrdersCount > 0 ? totalRevenue / completedOrdersCount : 0;

            // Daily revenue breakdown
            var dailyRevenue = completedOrders
                .Where(o => o.OrderDate.HasValue)
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Order status summary
            var statusSummary = orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new OrderStatusSummaryDto
                {
                    Status = g.Key ?? "Unknown",
                    Count = g.Count(),
                    TotalAmount = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            return new RevenueReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                CompletedOrders = completedOrdersCount,
                CancelledOrders = cancelledOrdersCount,
                AverageOrderValue = averageOrderValue,
                DailyRevenueBreakdown = dailyRevenue,
                OrderStatusSummary = statusSummary
            };
        }

        public OrderStatusesDto GetOrderStatuses()
        {
            return new OrderStatusesDto
            {
                OrderStatuses = new List<string> { "pending", "confirmed", "preparing", "delivered", "completed", "cancelled" },
                PaymentStatuses = new List<string> { "pending", "paid", "failed" },
                PaymentMethods = new List<string> { "cash", "card", "digital_wallet" }
            };
        }
           public async Task<OrderItemDto?> AddItemToOrderAsync(int orderId, AddOrderItemDto addItemDto)
        {
            // Validate order exists and can be modified
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return null;

            // Validate product exists
            var product = await _unitOfWork.Products.GetByIdAsync(addItemDto.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID {addItemDto.ProductId} does not exist");

            if (addItemDto.Quantity <= 0)
                throw new ArgumentException("Product quantity must be greater than 0");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create order item
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = addItemDto.ProductId,
                    Quantity = addItemDto.Quantity,
                    UnitPrice = product.BasePrice,
                    Subtotal = product.BasePrice * addItemDto.Quantity
                };

                await _unitOfWork.OrderItems.AddAsync(orderItem);
                await _unitOfWork.CompleteAsync(); // Save to get OrderItemId

                decimal toppingTotal = 0;

                // Process toppings if any
                if (addItemDto.Toppings != null && addItemDto.Toppings.Any())
                {
                    foreach (var toppingDto in addItemDto.Toppings)
                    {
                        // Validate topping exists
                        var topping = await _unitOfWork.Toppings.GetByIdAsync(toppingDto.ToppingId);
                        if (topping == null)
                            throw new ArgumentException($"Topping with ID {toppingDto.ToppingId} does not exist");

                        if (toppingDto.Quantity <= 0)
                            throw new ArgumentException("Topping quantity must be greater than 0");

                        var orderItemTopping = new OrderItemTopping
                        {
                            OrderItemId = orderItem.OrderItemId,
                            ToppingId = toppingDto.ToppingId,
                            Quantity = toppingDto.Quantity,
                            UnitPrice = topping.Price,
                            Subtotal = topping.Price * toppingDto.Quantity
                        };

                        await _unitOfWork.OrderItemTopping.AddAsync(orderItemTopping);
                        toppingTotal += orderItemTopping.Subtotal;
                    }
                }

                // Update order total amount
                order.TotalAmount += (orderItem.Subtotal + toppingTotal);
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return the created order item with full details
                var createdItem = await _unitOfWork.OrderItems.GetQueryable()
                    .Include(oi => oi.Product)
                    .Include(oi => oi.OrderItemToppings)
                        .ThenInclude(oit => oit.Topping)
                    .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItem.OrderItemId);

                return _mapper.Map<OrderItemDto>(createdItem);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> RemoveItemFromOrderAsync(int orderId, int itemId)
        {
            // Validate order exists and can be modified
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return false;

            // Get order item with toppings
            var orderItem = await _unitOfWork.OrderItems.GetQueryable()
                .Include(oi => oi.OrderItemToppings)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == itemId && oi.OrderId == orderId);

            if (orderItem == null)
                return false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Calculate total amount to subtract
                decimal amountToSubtract = orderItem.Subtotal;
                if (orderItem.OrderItemToppings != null && orderItem.OrderItemToppings.Any())
                {
                    amountToSubtract += orderItem.OrderItemToppings.Sum(oit => oit.Subtotal);
                }

                // Remove order item (toppings will be removed by cascade delete)
                _unitOfWork.OrderItems.Remove(orderItem);

                // Update order total amount
                order.TotalAmount -= amountToSubtract;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<OrderItemDto?> UpdateOrderItemAsync(int orderId, int itemId, UpdateOrderItemDto updateItemDto)
        {
            // Validate order exists and can be modified
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return null;

            // Get order item with toppings
            var orderItem = await _unitOfWork.OrderItems.GetQueryable()
                .Include(oi => oi.Product)
                .Include(oi => oi.OrderItemToppings)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == itemId && oi.OrderId == orderId);

            if (orderItem == null)
                return null;

            if (updateItemDto.Quantity <= 0)
                throw new ArgumentException("Product quantity must be greater than 0");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Calculate old total (item + toppings)
                decimal oldTotal = orderItem.Subtotal;
                if (orderItem.OrderItemToppings != null && orderItem.OrderItemToppings.Any())
                {
                    oldTotal += orderItem.OrderItemToppings.Sum(oit => oit.Subtotal);
                }

                // Update quantity and subtotal
                orderItem.Quantity = updateItemDto.Quantity;
                orderItem.Subtotal = orderItem.UnitPrice * (updateItemDto.Quantity ?? 0);

                // Remove existing toppings
                if (orderItem.OrderItemToppings != null && orderItem.OrderItemToppings.Any())
                {
                    foreach (var topping in orderItem.OrderItemToppings.ToList())
                    {
                        _unitOfWork.OrderItemTopping.Remove(topping);
                    }
                }

                decimal newToppingTotal = 0;

                // Add new toppings
                if (updateItemDto.Toppings != null && updateItemDto.Toppings.Any())
                {
                    foreach (var toppingDto in updateItemDto.Toppings)
                    {
                        // Validate topping exists
                        var topping = await _unitOfWork.Toppings.GetByIdAsync(toppingDto.ToppingId);
                        if (topping == null)
                            throw new ArgumentException($"Topping with ID {toppingDto.ToppingId} does not exist");

                        if (toppingDto.Quantity <= 0)
                            throw new ArgumentException("Topping quantity must be greater than 0");

                        var orderItemTopping = new OrderItemTopping
                        {
                            OrderItemId = orderItem.OrderItemId,
                            ToppingId = toppingDto.ToppingId,
                            Quantity = toppingDto.Quantity,
                            UnitPrice = topping.Price,
                            Subtotal = topping.Price * toppingDto.Quantity
                        };

                        await _unitOfWork.OrderItemTopping.AddAsync(orderItemTopping);
                        newToppingTotal += orderItemTopping.Subtotal;
                    }
                }

                // Update order item
                _unitOfWork.OrderItems.Update(orderItem);

                // Calculate new total
                decimal newTotal = orderItem.Subtotal + newToppingTotal;

                // Update order total amount
                order.TotalAmount = order.TotalAmount - oldTotal + newTotal;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return updated order item with full details
                var updatedItem = await _unitOfWork.OrderItems.GetQueryable()
                    .Include(oi => oi.Product)
                    .Include(oi => oi.OrderItemToppings)
                        .ThenInclude(oit => oit.Topping)
                    .FirstOrDefaultAsync(oi => oi.OrderItemId == itemId);

                return _mapper.Map<OrderItemDto>(updatedItem);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        // ===== OrderCombo Management =====

        public async Task<OrderComboDto?> AddComboToOrderAsync(int orderId, AddOrderComboDto addComboDto)
        {
            // Validate order exists and can be modified
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return null;

            // Validate combo exists
            var combo = await _unitOfWork.Combos.GetByIdAsync(addComboDto.ComboId);
            if (combo == null)
                throw new ArgumentException($"Combo with ID {addComboDto.ComboId} does not exist");

            if (addComboDto.Quantity <= 0)
                throw new ArgumentException("Combo quantity must be greater than 0");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create order combo
                var orderCombo = new OrderCombo
                {
                    OrderId = orderId,
                    ComboId = addComboDto.ComboId,
                    Quantity = addComboDto.Quantity,
                    UnitPrice = combo.ComboPrice,
                    Subtotal = combo.ComboPrice * addComboDto.Quantity
                };

                await _unitOfWork.OrderCombos.AddAsync(orderCombo);

                // Update order total amount
                order.TotalAmount += orderCombo.Subtotal;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return the created order combo with full details
                var createdCombo = await _unitOfWork.OrderCombos.GetQueryable()
                    .Include(oc => oc.Combo)
                    .FirstOrDefaultAsync(oc => oc.OrderComboId == orderCombo.OrderComboId);

                return _mapper.Map<OrderComboDto>(createdCombo);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> RemoveComboFromOrderAsync(int orderId, int orderComboId)
        {
            // Validate order exists and can be modified
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "pending")
                return false;

            // Get order combo
            var orderCombo = await _unitOfWork.OrderCombos.GetQueryable()
                .FirstOrDefaultAsync(oc => oc.OrderComboId == orderComboId && oc.OrderId == orderId);

            if (orderCombo == null)
                return false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Remove order combo
                _unitOfWork.OrderCombos.Remove(orderCombo);

                // Update order total amount
                order.TotalAmount -= orderCombo.Subtotal;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<bool> PrepareOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.OrderStatus != "confirmed")
                return false;

            order.OrderStatus = "preparing";
   

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeliverOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || (order.OrderStatus != "preparing" ))
                return false;

            order.OrderStatus = "ready"; 
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}