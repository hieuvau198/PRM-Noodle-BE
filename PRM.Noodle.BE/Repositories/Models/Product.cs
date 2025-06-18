using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsAvailable { get; set; }

    public string? SpiceLevel { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
