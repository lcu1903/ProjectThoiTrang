using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;
        public List<Models.Product> Products { get; set; } = new List<Models.Product>();
        public IndexModel(WebFashionContext context)
        {
            _context = context;

        }
        public async Task OnGetAsync()
        {
            Products = await _context.Products.Where(p=>p.Active == true).ToListAsync();
        }
    }
}
