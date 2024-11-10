using System;
using System.Collections.Generic;

namespace ProjectThoiTrang.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public int? CustomerId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public string? TransactionStatus { get; set; }

    public bool? Deleted { get; set; }

    public bool? Paid { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
