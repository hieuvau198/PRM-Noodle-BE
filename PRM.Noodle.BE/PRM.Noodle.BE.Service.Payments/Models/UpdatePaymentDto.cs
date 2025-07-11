using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Payments.Models
{
    public class UpdatePaymentDto
    {
        public int? CustomerUserId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }

        public int? StaffUserId { get; set; }

        [StringLength(100)]
        public string? StaffName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
        public decimal? PaymentAmount { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        public string? TransactionReference { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class PaymentStatusUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = null!;

        [StringLength(200)]
        public string? TransactionReference { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class PaymentDeleteDto
    {
        [Required]
        [StringLength(500)]
        public string DeletionReason { get; set; } = null!;
    }

    public class VnPayPaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        public int? CustomerUserId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }
    }

    public class VnPayCallbackDto
    {
        public string vnp_TxnRef { get; set; } = null!;
        public string vnp_ResponseCode { get; set; } = null!;
        public string vnp_TransactionStatus { get; set; } = null!;
        public string vnp_Amount { get; set; } = null!;
        public string vnp_BankCode { get; set; } = null!;
        public string vnp_PayDate { get; set; } = null!;
        public string vnp_TransactionNo { get; set; } = null!;
        public string vnp_SecureHash { get; set; } = null!;
    }

}
