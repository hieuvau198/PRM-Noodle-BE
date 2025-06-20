﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Combo
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

    public class UpdateComboDto
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

        public bool? IsAvailable { get; set; }
    }

    public class ComboIsAvailableDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}
