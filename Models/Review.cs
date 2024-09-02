using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Review
{
    public int RevId { get; set; }

    public int BookId { get; set; }

    public int Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CusId { get; set; }

    public string? Answer { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Customer Cus { get; set; } = null!;
}
