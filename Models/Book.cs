using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? BookName { get; set; }

    public string? AuthorName { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public string? Image { get; set; }

    public int? Nxbid { get; set; }

    public int? GenreId { get; set; }

    public int? Stock { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Genre? Genre { get; set; }

    public virtual Nxb? Nxb { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
