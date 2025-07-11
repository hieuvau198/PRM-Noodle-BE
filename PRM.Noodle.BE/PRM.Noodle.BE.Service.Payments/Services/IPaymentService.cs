using PRM.Noodle.BE.Service.Payments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Payments.Services
{
    public interface IPaymentService
    {
        // Basic CRUD operations
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<(IEnumerable<PaymentDto> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string searchTerm = null, string paymentStatus = null,
            string paymentMethod = null, int? orderId = null, int? customerId = null);
        Task<PaymentDto> GetByIdAsync(int id);
        Task<PaymentDto> CreateAsync(CreatePaymentDto dto);
        Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id, PaymentDeleteDto dto);

        // Payment-specific operations
        Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<PaymentDto>> GetByPaymentStatusAsync(string paymentStatus);
        Task<IEnumerable<PaymentDto>> GetByPaymentMethodAsync(string paymentMethod);
        Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Status management
        Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatusUpdateDto dto);
        Task<bool> MarkAsProcessedAsync(int id, string transactionReference = null);
        Task<bool> MarkAsCompletedAsync(int id, string transactionReference = null);
        Task<bool> MarkAsFailedAsync(int id, string reason = null);

        // VNPay integration
        Task<(PaymentDto Payment, string PaymentUrl)> CreateVnPayPaymentAsync(VnPayPaymentDto dto);
        Task<PaymentDto> HandleVnPayCallbackAsync(VnPayCallbackDto callback);
        Task<PaymentDto> GetByTransactionReferenceAsync(string transactionReference);

        // Analytics
        Task<decimal> GetTotalPaymentAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetPaymentMethodStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetPaymentStatusStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
