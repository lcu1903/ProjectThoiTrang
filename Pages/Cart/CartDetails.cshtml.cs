using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.RequestModel;
using ProjectThoiTrang.Service;

namespace ProjectThoiTrang.Pages.Cart
{
    public class CartDetailsModel : PageModel
    {
        private readonly WebFashionContext _context;        

        public ICollection<CartDetail> CartDetails { get; private set; }
        public CartDetailsModel(WebFashionContext context)
        {
            _context = context;            
        }

        public async Task<IActionResult> OnGetAsync(int userId)
        {
            var cart = await _context.Carts
                    .Where(c => c.CusId == userId && c.Paid !=true) 
                    .Include(c => c.CartDetails)  
                    .ThenInclude(cd => cd.Product)
                    .FirstOrDefaultAsync(); 

            if (cart == null)
            {
                cart = new Models.Cart { CusId = userId, Paid = false };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                return Page();
            }

            CartDetails = cart.CartDetails; 
            return Page();
        }
        
    }
}
