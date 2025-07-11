using System;
using System.Collections.Generic;

namespace PRM.Noodle.BE.Share.Models;

public partial class Payment
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

    public virtual User? CustomerUser { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User? StaffUser { get; set; }
}
