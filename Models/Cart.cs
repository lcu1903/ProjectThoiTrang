using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? CusId { get; set; }

    public bool? Paid { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Customer? Cus { get; set; }
}
