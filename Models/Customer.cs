using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Customer
{
    public int CusId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool Active { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Password { get; set; } = null!;

    public string? Sex { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
