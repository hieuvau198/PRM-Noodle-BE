using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRM.Noodle.BE.Service.Payments.Models;
using PRM.Noodle.BE.Service.Payments.Services;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    //[HttpGet("CreatePaymentUrl")]
    //public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
    //{
    //    VnpayPayment vnpayPayment = new VnpayPayment();
    //    try
    //    {
    //        var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

    //        var request = new PaymentRequest
    //        {
    //            PaymentId = DateTime.Now.Ticks,
    //            Money = moneyToPay,
    //            Description = description,
    //            IpAddress = ipAddress,
    //            //BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch

    //            CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
    //            Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
    //            Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
    //        };

    //        var paymentUrl = vnpayPayment._vnpay.GetPaymentUrl(request);

    //        return Created(paymentUrl, paymentUrl);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        // GET: api/payments/paged
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetPagedPayments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? paymentStatus = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] int? orderId = null,
            [FromQuery] int? customerId = null)
        {
            var result = await _paymentService.GetPagedAsync(page, pageSize, searchTerm, paymentStatus, paymentMethod, orderId, customerId);
            return Ok(new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize)
            });
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");

            return Ok(payment);
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        // PUT: api/payments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDto>> UpdatePayment(int id, UpdatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _paymentService.UpdateAsync(id, dto);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");

            return Ok(payment);
        }

        // DELETE: api/payments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // DELETE: api/payments/{id}/soft
        [HttpDelete("{id}/soft")]
        public async Task<ActionResult> SoftDeletePayment(int id, PaymentDeleteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.SoftDeleteAsync(id, dto);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // GET: api/payments/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByOrderId(int orderId)
        {
            var payments = await _paymentService.GetByOrderIdAsync(orderId);
            return Ok(payments);
        }

        // GET: api/payments/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByCustomerId(int customerId)
        {
            var payments = await _paymentService.GetByCustomerIdAsync(customerId);
            return Ok(payments);
        }

        // GET: api/payments/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByStatus(string status)
        {
            var payments = await _paymentService.GetByPaymentStatusAsync(status);
            return Ok(payments);
        }

        // GET: api/payments/method/{method}
        [HttpGet("method/{method}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByMethod(string method)
        {
            var payments = await _paymentService.GetByPaymentMethodAsync(method);
            return Ok(payments);
        }

        // GET: api/payments/date-range
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var payments = await _paymentService.GetByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }

        // PATCH: api/payments/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdatePaymentStatus(int id, PaymentStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.UpdatePaymentStatusAsync(id, dto);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // PATCH: api/payments/{id}/process
        [HttpPatch("{id}/process")]
        public async Task<ActionResult> MarkAsProcessed(int id, [FromBody] string? transactionReference = null)
        {
            var result = await _paymentService.MarkAsProcessedAsync(id, transactionReference);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // PATCH: api/payments/{id}/complete
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult> MarkAsCompleted(int id, [FromBody] string? transactionReference = null)
        {
            var result = await _paymentService.MarkAsCompletedAsync(id, transactionReference);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // PATCH: api/payments/{id}/fail
        [HttpPatch("{id}/fail")]
        public async Task<ActionResult> MarkAsFailed(int id, [FromBody] string? reason = null)
        {
            var result = await _paymentService.MarkAsFailedAsync(id, reason);
            if (!result)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }

        // GET: api/payments/transaction/{transactionReference}
        [HttpGet("transaction/{transactionReference}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentByTransactionReference(string transactionReference)
        {
            var payment = await _paymentService.GetByTransactionReferenceAsync(transactionReference);
            if (payment == null)
                return NotFound($"Payment with transaction reference {transactionReference} not found");

            return Ok(payment);
        }

        // VNPay Integration

        // POST: api/payments/vnpay/create
        [HttpPost("vnpay/create")]
        public async Task<ActionResult<object>> CreateVnPayPayment(VnPayPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _paymentService.CreateVnPayPaymentAsync(dto);
                return Ok(new
                {
                    Payment = result.Payment,
                    PaymentUrl = result.PaymentUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/payments/vnpay/callback
        [HttpPost("vnpay/callback")]
        public async Task<ActionResult<PaymentDto>> HandleVnPayCallback(VnPayCallbackDto callback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.HandleVnPayCallbackAsync(callback);
                if (payment == null)
                    return NotFound("Payment not found for this transaction");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/payments/vnpay/return
        [HttpGet("vnpay/return")]
        public async Task<ActionResult> HandleVnPayReturn([FromQuery] VnPayCallbackDto callback)
        {
            try
            {
                var payment = await _paymentService.HandleVnPayCallbackAsync(callback);
                if (payment == null)
                    return NotFound("Payment not found");

                // Redirect to success/failure page based on payment status
                if (payment.PaymentStatus == "Completed")
                {
                    return Redirect($"/payment/success?paymentId={payment.PaymentId}");
                }
                else
                {
                    return Redirect($"/payment/failed?paymentId={payment.PaymentId}");
                }
            }
            catch (Exception ex)
            {
                return Redirect($"/payment/error?message={ex.Message}");
            }
        }

        // Analytics endpoints

        // GET: api/payments/analytics/total-amount
        [HttpGet("analytics/total-amount")]
        public async Task<ActionResult<object>> GetTotalPaymentAmount(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var totalAmount = await _paymentService.GetTotalPaymentAmountAsync(startDate, endDate);
            return Ok(new { TotalAmount = totalAmount });
        }

        // GET: api/payments/analytics/method-statistics
        [HttpGet("analytics/method-statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetPaymentMethodStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var statistics = await _paymentService.GetPaymentMethodStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }

        // GET: api/payments/analytics/status-statistics
        [HttpGet("analytics/status-statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetPaymentStatusStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var statistics = await _paymentService.GetPaymentStatusStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
    
}