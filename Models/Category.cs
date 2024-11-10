using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? Catname { get; set; }

    public bool? Published { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
