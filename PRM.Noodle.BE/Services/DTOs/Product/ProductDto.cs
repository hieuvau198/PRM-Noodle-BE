using System;
using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Product
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsAvailable { get; set; }
        public string? SpiceLevel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProductDto
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

        public bool? IsAvailable { get; set; } = true;

        [StringLength(50)]
        public string? SpiceLevel { get; set; }
    }

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
    public class ProductIsAvailableDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}