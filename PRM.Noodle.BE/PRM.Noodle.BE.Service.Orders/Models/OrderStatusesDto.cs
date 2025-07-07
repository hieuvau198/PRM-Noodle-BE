using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class OrderStatusesDto
    {
        public List<string> OrderStatuses { get; set; } = new List<string>();
        public List<string> PaymentStatuses { get; set; } = new List<string>();
        public List<string> PaymentMethods { get; set; } = new List<string>();
    }
}
