using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class CartDetail
{
    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? CreateDate { get; set; }

    public decimal? Price { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
