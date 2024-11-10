using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminRoles
{
    public class DeleteModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notify;
        [BindProperty]
        public Role Role { get; set; } = new Role();
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

            Role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);

            if (Role == null)
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

            Role role = await _context.Roles.FindAsync(id);

            if (role != null)
            {
                _context.Roles.Remove(role);                
                _notify.Success("Xóa thành công");
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
