using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Products.Models
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than 0")]
        public decimal BasePrice { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public bool? IsAvailable { get; set; }

        [StringLength(50)]
        public string? SpiceLevel { get; set; }
    }
}
