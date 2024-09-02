using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class CartDetail
{
    public int CartDetailId { get; set; }

    public int Scid { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ShoppingCart Sc { get; set; } = null!;
}
