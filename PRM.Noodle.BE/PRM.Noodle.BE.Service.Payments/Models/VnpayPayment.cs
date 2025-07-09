using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPAY.NET;

namespace PRM.Noodle.BE.Service.Payments.Models
{
    public class VnpayPayment
    {
        private string _tmnCode;
        private string _hashSecret;
        private string _baseUrl;
        private string _callbackUrl;

        public readonly IVnpay _vnpay;

        public VnpayPayment()
        {
            // Khởi tạo giá trị cho _tmnCode, _hashSecret, _baseUrl, _callbackUrl tại đây.
            //_tmnCode = "UQILB436";
            //_hashSecret = "PRYYMYPQXXJKQVXAJTWLCRPJERZXSNWN";
            _tmnCode = "WKQTPQQX";
            _hashSecret = "P1Q8AZS0HQY13C1S4K492FV1FN4ZOQ48";
            _baseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            _callbackUrl = "https://prmnoodle.azurewebsites.net/index.html";
            _vnpay = new Vnpay();
            _vnpay.Initialize(_tmnCode, _hashSecret, _baseUrl, _callbackUrl);
        }
    }

}
