using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Payments.Models
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        public int? CustomerUserId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }

        public int? StaffUserId { get; set; }

        [StringLength(100)]
        public string? StaffName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
        public decimal PaymentAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        public string? TransactionReference { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
