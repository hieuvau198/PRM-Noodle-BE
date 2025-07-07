using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class CreateOrderItemDto
    {

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Danh sách topping cho sản phẩm này
        public List<CreateOrderItemToppingDto>? Toppings { get; set; } = new List<CreateOrderItemToppingDto>();
    }
}
