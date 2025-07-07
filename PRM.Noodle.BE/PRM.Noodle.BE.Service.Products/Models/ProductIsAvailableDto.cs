using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Products.Models
{
    public class ProductIsAvailableDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}
