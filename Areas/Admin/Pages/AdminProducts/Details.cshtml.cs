using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminProducts
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;

        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
