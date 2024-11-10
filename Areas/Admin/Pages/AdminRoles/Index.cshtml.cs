using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminRoles
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;        
        public List<Role> Roles { get; set;} = new List<Role>();
        public IndexModel(WebFashionContext context)
        {
            _context = context;
            
        }
        public async Task OnGetAsync()
        {
            Roles = await _context.Roles.ToListAsync();
        }
    }
}
