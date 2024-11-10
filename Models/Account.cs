using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Account
{
    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public bool? Active { get; set; }

    public int? RoleId { get; set; }

    public DateOnly? CreateDate { get; set; }

    public int AccountId { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual Role? Role { get; set; }
}
