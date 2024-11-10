using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminCategories
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;

        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }       

        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Categories.FirstOrDefaultAsync(m => m.CatId == id);
            if (Category == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
