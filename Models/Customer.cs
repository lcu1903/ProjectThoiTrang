using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Customer
{
    public int CusId { get; set; }

    public string? Fullname { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateOnly? CreateDate { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Account? EmailNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
