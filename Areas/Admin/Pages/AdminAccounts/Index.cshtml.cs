using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminAccounts
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;

        public SelectList QuyenTruyCap { get; set; }
        public SelectList TrangThai { get; set; }
        public PaginatedList<@Models.Account> Accounts { get; set; }

        public int CurrentPage { get; set; }
        public int CurrentRole { get; set; }
        public int CurrentActive { get; set; }

        public IndexModel(WebFashionContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? pageIndex, int roleID = 0, int active = 0)
        {            
            CurrentRole = roleID;
            CurrentActive = active;
            var pageSize = Helper.Help.page_size;
            IQueryable<@Models.Account> query = _context.Accounts.Include(a => a.Role);

            if (roleID > 0)
            {
                query = query.Where(x => x.RoleId == roleID);
            }
            if (active > 0)
            {
                bool isActive = active == 1;
                query = query.Where(x => x.Active == isActive);
            }
            Accounts = await PaginatedList<@Models.Account>.CreateAsync(query, pageIndex ?? 1, pageSize);           
            var quyentruycap = await _context.Roles.ToListAsync();
            quyentruycap.Insert(0, new Role { RoleId = 0, Description = "Chọn tất cả" });
            QuyenTruyCap = new SelectList(quyentruycap, "RoleId", "Description");
            TrangThai = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Chọn tất cả", Value = "0" },
                new SelectListItem { Text = "Kích hoạt", Value = "1" },
                new SelectListItem { Text = "Vô hiệu hóa", Value = "2" }
            }, "Value", "Text");
        }

        public JsonResult OnGetFilter(int roleID = 0, int active = 0)
        {
            var url = Url.Page("./Index", new { page = 1, roleID, active });
            return new JsonResult(new { status = "success", targetURL = url });
        }
    }
}
