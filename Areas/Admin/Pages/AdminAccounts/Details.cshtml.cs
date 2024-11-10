using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminAccounts
{
    public class DetailsModel : PageModel
    {
        private readonly WebFashionContext _context;

        public DetailsModel(WebFashionContext context)
        {
            _context = context;
        }

        public @Models.Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return NotFound();
            }

            Account = await _context.Accounts.FirstOrDefaultAsync(m => m.Email == email);
            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
