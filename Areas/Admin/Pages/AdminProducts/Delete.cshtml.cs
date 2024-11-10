using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminProducts
{
    public class DeleteModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notify;
        [BindProperty]
        public Product Product { get; set; } = new Product();
        public DeleteModel(WebFashionContext context, INotyfService notify)
        {
            _context = context;
            _notify = notify;
        }

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

        public async Task<IActionResult> OnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product Product = await _context.Products.FindAsync(id);

            if (Product != null)
            {
                _context.Products.Remove(Product);
                _notify.Success("Xóa thành công");
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
