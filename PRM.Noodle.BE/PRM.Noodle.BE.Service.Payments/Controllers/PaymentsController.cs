using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRM.Noodle.BE.Service.Payments.Models;
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
    [HttpGet("create-payment")]
    public IActionResult CreatePayment()
    {
        // Thông tin sandbox VNPay (thay bằng thông tin thật khi có)
        string vnp_TmnCode = "UQILB436"; // Test TMN code
        string vnp_HashSecret = "PRYYMYPQXXJKQVXAJTWLCRPJERZXSNWN"; // Test secret key
        string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        string vnp_Returnurl = "https://prmnoodle.azurewebsites.net"; // URL callback

        // Tạo thông tin giao dịch
        string vnp_TxnRef = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        string vnp_OrderInfo = "Nap 100K cho so dienthoai 0934998386";
        string vnp_OrderType = "other";
        string vnp_Amount = (10000 * 100).ToString(); // 10,000 VND * 100
        string vnp_Locale = "vn";
        string vnp_IpAddr = "127.0.0.1";
        string vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        // Build parameters
        var vnp_Params = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", vnp_Amount },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", vnp_TxnRef },
            { "vnp_OrderInfo", vnp_OrderInfo },
            { "vnp_OrderType", vnp_OrderType },
            { "vnp_Locale", vnp_Locale },
            { "vnp_ReturnUrl", vnp_Returnurl },
            { "vnp_IpAddr", vnp_IpAddr },
            { "vnp_CreateDate", vnp_CreateDate }
        };

        // Sort parameters by key (alphabet order)
        var sorted = vnp_Params
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .OrderBy(x => x.Key)
            .ToList();

        // Build hash data (NO URL encoding)
        var hashData = string.Join("&", sorted.Select(x => $"{x.Key}={x.Value}"));

        // Build query string (WITH URL encoding)
        var query = string.Join("&", sorted.Select(x =>
            $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}"
        ));

        // Create secure hash
        string vnp_SecureHash = HmacSHA512(vnp_HashSecret, hashData);

        // Build final payment URL
        var paymentUrl = $"{vnp_Url}?{query}&vnp_SecureHash={vnp_SecureHash}";

        return Ok(new
        {
            paymentUrl = paymentUrl,
            txnRef = vnp_TxnRef,
            amount = vnp_Amount,
            orderInfo = vnp_OrderInfo
        });
    }

    [HttpGet("return")]
    public IActionResult PaymentReturn()
    {
        // Xử lý kết quả trả về từ VNPay
        var vnp_HashSecret = "PRYYMYPQXXJKQVXAJTWLCRPJERZXSNWN"; // Same secret key
        var vnp_SecureHash = Request.Query["vnp_SecureHash"];

        // Lấy tất cả parameters trừ vnp_SecureHash
        var vnp_Params = Request.Query
            .Where(x => x.Key != "vnp_SecureHash")
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        // Sort và tạo hash data
        var sorted = vnp_Params
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .OrderBy(x => x.Key)
            .ToList();

        var hashData = string.Join("&", sorted.Select(x => $"{x.Key}={x.Value}"));
        var checkSum = HmacSHA512(vnp_HashSecret, hashData);

        if (checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase))
        {
            // Chữ ký hợp lệ
            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
            var vnp_TxnRef = Request.Query["vnp_TxnRef"];
            var vnp_Amount = Request.Query["vnp_Amount"];

            if (vnp_ResponseCode == "00")
            {
                // Giao dịch thành công
                return Ok(new
                {
                    status = "success",
                    message = "Paymentsuccessful",
                    txnRef = vnp_TxnRef,
                    amount = vnp_Amount
                });
            }
            else
            {
                // Giao dịch thất bại
                return Ok(new
                {
                    status = "failed",
                    message = "Paymentfailed",
                    responseCode = vnp_ResponseCode
                });
            }
        }
        else
        {
            // Chữ ký không hợp lệ
            return BadRequest(new
            {
                status = "error",
                message = "Invalidsignature"
            });
        }
    }

    private string GetClientIpAddress()
    {
        // Lấy IP từ X-Forwarded-For header (nếu có proxy)
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Lấy IP từ X-Real-IP header
        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Lấy IP từ RemoteIpAddress
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }

    private static string HmacSHA512(string key, string inputData)
    {
        using var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    [HttpGet("CreatePaymentUrl")]
    public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
    {
        VnpayPayment vnpayPayment = new VnpayPayment();
        try
        {
            var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

            var request = new PaymentRequest
            {
                PaymentId = DateTime.Now.Ticks,
                Money = moneyToPay,
                Description = description,
                IpAddress = ipAddress,
                //BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                
                CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
            };
            
            var paymentUrl = vnpayPayment._vnpay.GetPaymentUrl(request);

            return Created(paymentUrl, paymentUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}