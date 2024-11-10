using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Productname { get; set; }

    public string? ShortDesc { get; set; }

    public string? Description { get; set; }

    public int? CatId { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    public string? Thumb { get; set; }

    public DateOnly? DateCreated { get; set; }

    public DateOnly? DateModified { get; set; }

    public bool? BestSellers { get; set; }

    public bool? Homeflag { get; set; }

    public bool? Active { get; set; }

    public string? Tags { get; set; }

    public int? Stock { get; set; }

    public string? Title { get; set; }

    public int? BrandId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
