using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class ShoppingCart
{
    public int Scid { get; set; }

    public int CusId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Customer Cus { get; set; } = null!;
}
