using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class AddOrderItemDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public List<CreateOrderItemToppingDto>? Toppings { get; set; } = new List<CreateOrderItemToppingDto>();
    }
}
