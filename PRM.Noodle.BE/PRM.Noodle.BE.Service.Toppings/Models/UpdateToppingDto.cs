using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Toppings.Models
{
    public class UpdateToppingDto
    {
        [Required]
        [StringLength(100)]
        public string ToppingName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
