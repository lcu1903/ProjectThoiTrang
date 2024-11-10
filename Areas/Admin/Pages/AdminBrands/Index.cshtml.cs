using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminBrands
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notify;
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public IndexModel(WebFashionContext context, INotyfService notify)
        {
            _context = context;
            _notify = notify;
        }
        public async Task OnGetAsync()
        {
            Brands = await _context.Brands.ToListAsync();
        }
    }
}
