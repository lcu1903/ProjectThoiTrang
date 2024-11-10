using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.RequestModel;
using ProjectThoiTrang.Service;

namespace ProjectThoiTrang.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly IVnPayService _vnPayService;
        public List<CartDetail> CartDetails { get; set; }

        public CheckoutModel(WebFashionContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        [BindProperty]
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int userId)
        {
            UserId = userId;
            Service.GetID.ID = userId;
            var cart = await _context.Carts
            .Include(c => c.CartDetails)
            .ThenInclude(cd => cd.Product)
            .FirstOrDefaultAsync(c => c.CusId == userId);
            if (cart != null)
            {                
                CartDetails = cart.CartDetails.ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string payment, int userId)
        {
            var cart = await _context.Carts
                .Where(c => c.CusId == userId && c.Paid != true)
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CusId == userId);
            if (cart == null || customer == null)
            {
                return NotFound();
            }

            if (payment == "Thanh toán VnPay")
            {                
                await _context.SaveChangesAsync();
                decimal totalAmount = (decimal)cart.CartDetails.Sum(cd => cd.Quantity * (cd.Product.Price - cd.Product.Price*cd.Product.Discount/100));
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (double)totalAmount,
                    CreatedDate = DateTime.Now,
                    Description = "Thanh toán đơn hàng",
                    OrderId = cart.CartId.ToString(),
                    CusId = customer.CusId,
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }            
            return RedirectToPage("/Cart/PaymentCallBack"); // Redirect to payment fail page if payment method is not supported
        }
    }
}
