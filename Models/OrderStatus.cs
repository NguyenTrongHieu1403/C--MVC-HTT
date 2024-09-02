using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class OrderStatus
{
    public int Osid { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
