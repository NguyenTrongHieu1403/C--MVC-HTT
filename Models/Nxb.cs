using System;
using System.Collections.Generic;

namespace H2TSHOP2024.Models;

public partial class Nxb
{
    public int Nxbid { get; set; }

    public string Nxbname { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
