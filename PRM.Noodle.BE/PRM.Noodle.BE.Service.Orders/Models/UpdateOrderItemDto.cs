using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class UpdateOrderItemDto
    {
        public int? Quantity { get; set; }

        public List<UpdateOrderItemToppingDto>? Toppings { get; set; }
    }
}
