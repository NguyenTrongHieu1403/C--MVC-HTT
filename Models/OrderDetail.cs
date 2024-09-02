using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class OrderDetail
{
    public int Odid { get; set; }

    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
