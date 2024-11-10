using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminCustomers
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;

        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }

        public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer = await _context.Customers.FirstOrDefaultAsync(m => m.CusId == id);
            if (Customer == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
