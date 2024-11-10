using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminCustomers
{
    public class EditModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notyf;

        public EditModel(WebFashionContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer = await _context.Customers.FindAsync(id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (id != Customer.CusId)
            {
                return NotFound();
            }

            _context.Attach(Customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _notyf.Success("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(Customer.CusId))
                {
                    _notyf.Error("Không tìm thấy khách hàng.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
