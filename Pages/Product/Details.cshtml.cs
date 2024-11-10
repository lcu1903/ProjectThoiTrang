using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Pages.Product
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;
        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }
        public Models.Product Product { get; set; }
        public List<Models.Product> lstProduct {  get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }            
            Product = await _context.Products.Include(x=>x.Cat).FirstOrDefaultAsync(m => m.ProductId == id);
            lstProduct =  _context.Products.AsNoTracking()
                .Where(x => x.CatId == Product.CatId  && x.ProductId != Product.CatId && x.Active ==true )
                .OrderByDescending(x=>x.DateCreated)
                .Take(4)
                .ToList();
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
