using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class ComboProduct
{
    public int ComboProductId { get; set; }

    public int ComboId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
