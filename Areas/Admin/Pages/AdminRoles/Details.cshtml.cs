using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminRoles
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;

        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }       

        public Role role { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
