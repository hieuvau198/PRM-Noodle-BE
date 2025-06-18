using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Topping
{
    public int ToppingId { get; set; }

    public string ToppingName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderItemTopping> OrderItemToppings { get; set; } = new List<OrderItemTopping>();
}
