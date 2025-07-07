using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Toppings.Models
{
    public class ToppingDto
    {
        public int ToppingId { get; set; }
        public string ToppingName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
