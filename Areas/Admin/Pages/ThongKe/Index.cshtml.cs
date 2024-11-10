using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.ThongKe
{
    public class IndexModel : PageModel
    {
		private readonly WebFashionContext _context; 

		public int tongDonHang { get; set; }
		public int tongKhachHang { get; set; }
		public int tongSpDaBan { get; set; }
		public decimal tongDoanhThu { get; set; }
		public int tongDonHanDaHoanTat { get; set; }
		public int tongDonHangDaHuy { get; set; }

		public IndexModel(WebFashionContext context)
		{
			_context = context;
		}

		public async Task OnGetAsync()
		{
			tongDonHang = await _context.Orders.CountAsync();
			tongKhachHang = await _context.Customers.CountAsync();
			tongSpDaBan = (int)await _context.OrderDetails.SumAsync(od => od.Quantity);
			tongDoanhThu = (decimal)await _context.OrderDetails.SumAsync(od => od.Price * od.Quantity);
			tongDonHanDaHoanTat = await _context.Orders.CountAsync(o => (bool)o.Paid);
			tongDonHangDaHuy = await _context.Orders.CountAsync(o => (bool)!o.Paid);
		}
	}
}
