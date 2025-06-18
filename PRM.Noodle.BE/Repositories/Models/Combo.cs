using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Combo
{
    public int ComboId { get; set; }

    public string ComboName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal ComboPrice { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();

    public virtual ICollection<OrderCombo> OrderCombos { get; set; } = new List<OrderCombo>();
}
