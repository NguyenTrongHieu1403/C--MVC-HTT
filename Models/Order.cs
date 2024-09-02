using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CusId { get; set; }

    public DateTime CreateDate { get; set; }

    public int OrderStatusId { get; set; }

    public bool IsDeleted { get; set; }

    public int LocationId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool IsPaid { get; set; }

    public virtual Customer Cus { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;
}
