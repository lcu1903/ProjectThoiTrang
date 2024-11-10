using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using static NuGet.Packaging.PackagingConstants;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminOrders
{
    public class IndexModel : PageModel
    {
		private readonly WebFashionContext _context;
        public List<Order> orders { get; set; }
        public IndexModel(WebFashionContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();

            foreach (var order in orders)
            {
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == order.OrderId)
                    .ToListAsync();

                order.OrderDetails = orderDetails;
            }
        }
    }
}
