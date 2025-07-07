using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Models
{
    public class ComboDto
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; }
        public string Description { get; set; }
        public decimal ComboPrice { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
