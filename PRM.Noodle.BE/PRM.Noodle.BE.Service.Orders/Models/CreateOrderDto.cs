using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class CreateOrderDto
    {

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [MaxLength(500, ErrorMessage = "Delivery address cannot exceed 500 characters")]
        public string? DeliveryAddress { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(cash|card|digital_wallet)$", ErrorMessage = "Payment method must be cash, card, or digital_wallet")]
        public string PaymentMethod { get; set; } = "cash";

        public List<CreateOrderItemDto>? OrderItems { get; set; } = new List<CreateOrderItemDto>();

        public List<CreateOrderComboDto>? OrderCombos { get; set; } = new List<CreateOrderComboDto>();

        public bool HasItems => (OrderItems?.Count > 0) || (OrderCombos?.Count > 0);
    }
}
