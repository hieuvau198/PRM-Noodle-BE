using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Payments.Models
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int? CustomerUserId { get; set; }
        public string? CustomerName { get; set; }
        public int? StaffUserId { get; set; }
        public string? StaffName { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? PaymentStatus { get; set; }
        public string? TransactionReference { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeletionReason { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
