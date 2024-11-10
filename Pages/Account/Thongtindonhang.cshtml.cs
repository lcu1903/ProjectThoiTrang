using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Pages.Account
{
    public class ThongtindonhangModel : PageModel
    {
        private readonly WebFashionContext _context;
        public ICollection<Order> Orders { get; private set; }
        public ThongtindonhangModel(WebFashionContext context) {
            _context = context;
        }
        public IActionResult OnGetAsync(int userId)
        {
            var orders =  _context.Orders
                    .Where(o => o.CustomerId == userId)  
                    .Include(o => o.OrderDetails)
                    .ThenInclude(cd => cd.Product)
                    .ToList();            
            if (orders == null)
            {
                return Page();
            }
            Orders = orders;
            foreach (var order in Orders)
            {
                var orderDetails = _context.OrderDetails
                    .Where(od => od.OrderId == order.OrderId)
                    .ToList();                
                order.OrderDetails = orderDetails;
            }
            return Page();
        }
    }
}
