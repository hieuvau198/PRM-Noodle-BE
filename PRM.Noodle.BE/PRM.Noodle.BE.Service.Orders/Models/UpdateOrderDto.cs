using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class UpdateOrderDto
    {
        [RegularExpression("^(pending|confirmed|preparing|ready|delivered|cancelled)$",
            ErrorMessage = "Invalid order status")]
        public string? OrderStatus { get; set; }

        [RegularExpression("^(pending|paid|failed)$",
            ErrorMessage = "Invalid payment status")]
        public string? PaymentStatus { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? Notes { get; set; }
    }
}
