using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public int TotalItems { get; set; }
    }
}
