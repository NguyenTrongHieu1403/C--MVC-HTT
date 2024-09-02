using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public int CusId { get; set; }

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public bool? IsDefault { get; set; }

    public virtual Customer Cus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
