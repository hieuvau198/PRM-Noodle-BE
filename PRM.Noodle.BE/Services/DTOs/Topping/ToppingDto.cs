//using System.ComponentModel.DataAnnotations;

//namespace Services.DTOs.Topping
//{
//    public class ToppingDto
//    {
//        public int ToppingId { get; set; }
//        public string ToppingName { get; set; }
//        public decimal Price { get; set; }
//        public string? Description { get; set; }
//        public bool? IsAvailable { get; set; }
//    }

//    public class CreateToppingDto
//    {
//        [Required]
//        [StringLength(100)]
//        public string ToppingName { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
//        public decimal Price { get; set; }

//        [StringLength(500)]
//        public string? Description { get; set; }

//        public bool? IsAvailable { get; set; } = true;
//    }

//    public class UpdateToppingDto
//    {
//        [Required]
//        [StringLength(100)]
//        public string ToppingName { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
//        public decimal Price { get; set; }

//        [StringLength(500)]
//        public string? Description { get; set; }

//        public bool? IsAvailable { get; set; }
//    }

//    public class ToppingIsAvailableDto
//    {
//        [Required]
//        public bool IsAvailable { get; set; }
//    }
//    public class PagedToppingResponse
//    {
//        public IEnumerable<ToppingDto> Items { get; set; }
//        public int TotalCount { get; set; }
//        public int Page { get; set; }
//        public int PageSize { get; set; }
//        public int TotalPages { get; set; }
//    }
//}
