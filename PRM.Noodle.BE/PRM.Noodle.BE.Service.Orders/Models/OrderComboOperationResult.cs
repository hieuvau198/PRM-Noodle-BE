using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class OrderComboOperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public OrderComboDto? OrderCombo { get; set; }
        public decimal UpdatedOrderTotal { get; set; }
    }
}
