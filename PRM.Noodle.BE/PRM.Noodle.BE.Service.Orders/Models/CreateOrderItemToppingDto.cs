using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class CreateOrderItemToppingDto
    {

        public int ToppingId { get; set; }

        public int Quantity { get; set; }
    }
}
