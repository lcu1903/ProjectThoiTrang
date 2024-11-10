using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Helper;
using System.Linq;
namespace ProjectThoiTrang.Areas.Admin.Pages.AdminCustomers
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;
        public PaginatedList<Customer> Customers { get; set; } 

        public IndexModel(WebFashionContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? pageIndex)
        {            
            var pageSize = Helper.Help.page_size;
            IQueryable<Customer> query = _context.Customers.AsNoTracking();
            Customers = await PaginatedList<Customer>.CreateAsync(query, pageIndex ?? 1, pageSize);
            return Page();
        }
    }
}