using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PRM.Noodle.BE.Service.Payments.Models;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace PRM.Noodle.BE.Service.Payments.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VnpayPayment _vnpayPayment;

        public PaymentService(
            IGenericRepository<Payment> paymentRepo,
            IUnitOfWork uow,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _paymentRepo = paymentRepo;
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _vnpayPayment = new VnpayPayment();
        }

        // Basic CRUD operations
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<(IEnumerable<PaymentDto> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string searchTerm = null, string paymentStatus = null,
            string paymentMethod = null, int? orderId = null, int? customerId = null)
        {
            var query = _paymentRepo.GetQueryable()
                .Where(p => p.IsDeleted != true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.CustomerName.Contains(searchTerm) ||
                    p.StaffName.Contains(searchTerm) ||
                    p.TransactionReference.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(p => p.PaymentStatus == paymentStatus);

            if (!string.IsNullOrEmpty(paymentMethod))
                query = query.Where(p => p.PaymentMethod == paymentMethod);

            if (orderId.HasValue)
                query = query.Where(p => p.OrderId == orderId);

            if (customerId.HasValue)
                query = query.Where(p => p.CustomerUserId == customerId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<PaymentDto>>(items), totalCount);
        }

        public async Task<PaymentDto> GetByIdAsync(int id)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            await _paymentRepo.AddAsync(payment);
            await _uow.CompleteAsync();
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto dto)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return null;

            _mapper.Map(dto, payment);
            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            _paymentRepo.Remove(payment);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, PaymentDeleteDto dto)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.IsDeleted = true;
            payment.DeletionReason = dto.DeletionReason;
            payment.UpdatedAt = DateTime.UtcNow;

            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return true;
        }

        // Payment-specific operations
        public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId)
        {
            var payments = await _paymentRepo.FindAsync(p => p.OrderId == orderId && p.IsDeleted != true);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetByCustomerIdAsync(int customerId)
        {
            var payments = await _paymentRepo.FindAsync(p => p.CustomerUserId == customerId && p.IsDeleted != true);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetByPaymentStatusAsync(string paymentStatus)
        {
            var payments = await _paymentRepo.FindAsync(p => p.PaymentStatus == paymentStatus && p.IsDeleted != true);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetByPaymentMethodAsync(string paymentMethod)
        {
            var payments = await _paymentRepo.FindAsync(p => p.PaymentMethod == paymentMethod && p.IsDeleted != true);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepo.FindAsync(p =>
                p.PaymentDate >= startDate &&
                p.PaymentDate <= endDate &&
                p.IsDeleted != true);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        // Status management
        public async Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatusUpdateDto dto)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.PaymentStatus = dto.PaymentStatus;
            payment.TransactionReference = dto.TransactionReference ?? payment.TransactionReference;
            payment.ProcessedAt = dto.ProcessedAt ?? payment.ProcessedAt;
            payment.CompletedAt = dto.CompletedAt ?? payment.CompletedAt;
            payment.UpdatedAt = DateTime.UtcNow;

            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> MarkAsProcessedAsync(int id, string transactionReference = null)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.PaymentStatus = "Processed";
            payment.ProcessedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(transactionReference))
                payment.TransactionReference = transactionReference;

            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(int id, string transactionReference = null)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.PaymentStatus = "Completed";
            payment.CompletedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(transactionReference))
                payment.TransactionReference = transactionReference;

            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> MarkAsFailedAsync(int id, string reason = null)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.PaymentStatus = "Failed";
            payment.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(reason))
                payment.DeletionReason = reason; // Using this field for failure reason

            _paymentRepo.Update(payment);
            await _uow.CompleteAsync();
            return true;
        }

        // VNPay integration
        public async Task<(PaymentDto Payment, string PaymentUrl)> CreateVnPayPaymentAsync(VnPayPaymentDto dto)
        {
            // Create payment record
            var createDto = _mapper.Map<CreatePaymentDto>(dto);
            var payment = _mapper.Map<Payment>(createDto);

            // Generate unique transaction reference
            var transactionRef = $"PAY_{DateTime.Now.Ticks}_{dto.OrderId}";
            payment.TransactionReference = transactionRef;

            await _paymentRepo.AddAsync(payment);
            await _uow.CompleteAsync();

            // Create VNPay payment URL
            var ipAddress = NetworkHelper.GetIpAddress(_httpContextAccessor.HttpContext);
            var request = new PaymentRequest
            {
                PaymentId = payment.PaymentId,
                Money = (double)dto.Amount,
                Description = dto.Description,
                IpAddress = ipAddress,
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            var paymentUrl = _vnpayPayment._vnpay.GetPaymentUrl(request);
            var paymentDto = _mapper.Map<PaymentDto>(payment);

            return (paymentDto, paymentUrl);
        }

        public async Task<PaymentDto> HandleVnPayCallbackAsync(VnPayCallbackDto callback)
        {
            var payment = await _paymentRepo.FindAsync(p => p.PaymentId.ToString() == callback.vnp_TxnRef);
            var paymentRecord = payment.FirstOrDefault();

            if (paymentRecord == null) return null;

            // Update payment based on VNPay response
            if (callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00")
            {
                paymentRecord.PaymentStatus = "Completed";
                paymentRecord.CompletedAt = DateTime.UtcNow;
                paymentRecord.ProcessedAt = DateTime.UtcNow;
            }
            else
            {
                paymentRecord.PaymentStatus = "Failed";
                paymentRecord.DeletionReason = $"VNPay Error: {callback.vnp_ResponseCode}";
            }

            paymentRecord.TransactionReference = callback.vnp_TransactionNo;
            paymentRecord.UpdatedAt = DateTime.UtcNow;

            _paymentRepo.Update(paymentRecord);
            await _uow.CompleteAsync();

            return _mapper.Map<PaymentDto>(paymentRecord);
        }

        public async Task<PaymentDto> GetByTransactionReferenceAsync(string transactionReference)
        {
            var payments = await _paymentRepo.FindAsync(p => p.TransactionReference == transactionReference);
            var payment = payments.FirstOrDefault();
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        // Analytics
        public async Task<decimal> GetTotalPaymentAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _paymentRepo.GetQueryable()
                .Where(p => p.IsDeleted != true && p.PaymentStatus == "Completed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate);

            return await query.SumAsync(p => p.PaymentAmount);
        }

        public async Task<Dictionary<string, int>> GetPaymentMethodStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _paymentRepo.GetQueryable()
                .Where(p => p.IsDeleted != true);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate);

            return await query
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new { Method = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Method, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetPaymentStatusStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _paymentRepo.GetQueryable()
                .Where(p => p.IsDeleted != true);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate);

            return await query
                .GroupBy(p => p.PaymentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status ?? "Unknown", x => x.Count);
        }
    }
}
