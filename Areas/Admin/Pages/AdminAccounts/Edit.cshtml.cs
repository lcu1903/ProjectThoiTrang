using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminAccounts
{
    public class EditModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notyf;
        public SelectList QuyenTruyCap { get; set; }
        public SelectList TrangThai { get; set; }

        public EditModel(WebFashionContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        [BindProperty]
        public @Models.Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return NotFound();
            }

            Account = await _context.Accounts.FindAsync(email);

            if (Account == null)
            {
                return NotFound();
            }
            QuyenTruyCap = new SelectList(_context.Roles, "RoleId", "Description");
            TrangThai = new SelectList(new List<SelectListItem>
            {                
                new SelectListItem { Text = "Kích hoạt", Value = "true" },
                new SelectListItem { Text = "Vô hiệu hóa", Value = "false" }
            }, "Value", "Text");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (email != Account.Email)
            {
                return NotFound();
            }
            
            _context.Attach(Account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _notyf.Success("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(Account.Email))
                {
                    _notyf.Error("Không tìm thấy tài khoản.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RoleExists(string email)
        {
            return _context.Accounts.Any(e => e.Email == email);
        }
    }
}
