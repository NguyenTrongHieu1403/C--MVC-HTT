using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Voucher
{
    public int VouId { get; set; }

    public string Code { get; set; } = null!;

    public double DiscountPer { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int CusId { get; set; }

    public bool IsUsed { get; set; }

    public bool IsActive { get; set; }

    public virtual Customer Cus { get; set; } = null!;
}
