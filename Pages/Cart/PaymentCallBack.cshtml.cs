using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Service;

namespace ProjectThoiTrang.Pages.Cart
{
    [Authorize]
    public class PaymentCallBackModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly IVnPayService _vnPayService;

        public PaymentCallBackModel(WebFashionContext context, IVnPayService vnPayService)
        {
            _context= context;
            _vnPayService = vnPayService;
        }

        public IActionResult OnGet()
        {
            var response = _vnPayService.PaymentExecute(Request.Query); 
            var cart = _context.Carts
                .Where(c => c.CusId == Service.GetID.ID)
                .Include(c => c.CartDetails)
                .FirstOrDefault();
            var cardetails = _context.CartDetails.Where(cd => cd.CartId == cart.CartId).ToList();
            if (response == null || response.VnPayResponseCode != "00")
            {                
                var order1 = new Order
                {
                    OrderId = response.OrderId,
                    Amount = (decimal)response.Amount,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Deleted = true,
                    TransactionStatus = response.TransactionId,
                    CustomerId = Service.GetID.ID,
                    Paid = false,
                };
                _context.Orders.Add(order1);
                foreach (var cd in cardetails)
                {
                    var orderdetail = new OrderDetail
                    {
                        OrderId = order1.OrderId,
                        ProductId = cd.ProductId,
                        Quantity = cd.Quantity,
                        Price = cd.Price
                    };
                    _context.OrderDetails.Add(orderdetail);                    
                }
                _context.RemoveRange(cardetails);

                _context.SaveChanges();
                return RedirectToPage("/PaymentFail");
            }
            var order = new Order
            {
                OrderId = response.OrderId,
                Amount = (decimal)response.Amount,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                Deleted = false,
                TransactionStatus = response.TransactionId,
                CustomerId = Service.GetID.ID,
                Paid = true,
            };
            _context.Orders.Add(order);
            
            foreach (var cd in cardetails)
            {
                var orderdetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = cd.ProductId,
                    Quantity = cd.Quantity,
                    Price = cd.Price
                };
                _context.OrderDetails.Add(orderdetail);
                var product = _context.Products.FirstOrDefault(p => p.ProductId == cd.ProductId);
                if (product != null)
                {
                    product.Stock -= cd.Quantity; // Giảm số lượng từ số lượng tồn kho
                }
            }
            _context.RemoveRange(cardetails);
            _context.SaveChanges();
            
            _context.SaveChanges();           
            return RedirectToPage("/PaymentSuccess");
        }
    }
}
