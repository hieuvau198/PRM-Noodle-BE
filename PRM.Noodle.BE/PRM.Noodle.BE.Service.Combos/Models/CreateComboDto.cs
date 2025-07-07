using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Models
{
    public class CreateComboDto
    {
        [Required]
        [StringLength(200)]
        public string ComboName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Combo price must be greater than 0")]
        public decimal ComboPrice { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public bool? IsAvailable { get; set; } = true;
    }
}
