using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class OrderItemToppingDto
    {
        public int OrderItemToppingId { get; set; }
        public int ToppingId { get; set; }
        public string? ToppingName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
