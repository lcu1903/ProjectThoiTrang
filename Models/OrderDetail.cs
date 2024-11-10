using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class OrderDetail
{
    public string OrderId { get; set; } = null!;

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
