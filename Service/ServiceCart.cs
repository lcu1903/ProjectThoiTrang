using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Service
{
    public class ServiceCart
    {
        private readonly WebFashionContext _context;
        

        public ServiceCart(WebFashionContext context)
        {
            _context = context;            
        }

        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var order = await _context.Carts
                .Where(o => o.CusId == userId && o.Paid != true)
                .Include(o => o.CartDetails)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                order = new Cart { CusId = userId,  Paid = false };
                _context.Carts.Add(order);
                await _context.SaveChangesAsync();
            }
            var product = _context.Products.Where(p => p.ProductId == productId).FirstOrDefault();
            var orderDetail = order.CartDetails.FirstOrDefault(od => od.ProductId == productId);
            if (orderDetail == null)
            {
                orderDetail = new CartDetail { ProductId = productId, Quantity = quantity, CreateDate =DateOnly.FromDateTime(DateTime.Now)};
                orderDetail.Price = quantity *( product.Price - product.Price * product.Discount/100);
                order.CartDetails.Add(orderDetail);
            }
            else
            {
                orderDetail.Quantity += quantity;
                if (orderDetail.Quantity > product.Stock)
                {
                    orderDetail.Quantity = product.Stock;
                    return new JsonResult(new { success = false });
                }                
            }
            orderDetail.Price = orderDetail.Quantity * (product.Price - product.Price*product.Discount/100);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        public async Task<IActionResult> GetCartDeTails(int userId)
        {
            var cart = await _context.Carts
                .Where(c => c.CusId == userId && c.Paid != true)
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync();                  
            if (cart == null || !cart.CartDetails.Any())
            {
                return new NotFoundObjectResult(new { Message = "Cart not found or no details available." });
            }

            var cartDetails = cart.CartDetails.Count();            
            return new JsonResult(new { success = true , cartamout = cartDetails }); ;
        }
        public async Task<IActionResult> DeleteProduct(int userId, int productId)
        {
            var cart = await _context.Carts
                .Where(o => o.CusId == userId && o.Paid != true)
                .Include(o => o.CartDetails)
                .FirstOrDefaultAsync();            
            var cartDetail = cart.CartDetails.FirstOrDefault(od => od.ProductId == productId);
            cart.CartDetails.Remove(cartDetail);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true }); ;
        }
    }
}
