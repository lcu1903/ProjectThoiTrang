using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using System.Drawing.Printing;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminProducts
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;           
        public IndexModel(WebFashionContext context)
        {
            _context = context;
        }
        public PaginatedList<Product> Products { get; set; }
        public SelectList DanhMuc { get; set; }
        public SelectList Brands { get; set; }
        public SelectList Categories { get; set; }
        public int CurrCat { get; set; }
        public int CurrentPage { get; set; }
        public async Task OnGetAsync(int? pageIndex , int catID)
        {
            var pageSize = Helper.Help.page_size;
            CurrCat = catID;
            IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Include(x => x.Cat)
            .Include(x => x.Brand);

            if (CurrCat > 0)
            {
                query = query.Where(x => x.CatId == catID);
            }

            Products = await PaginatedList<Product>.CreateAsync(query, pageIndex ?? 1, pageSize);

            var danhmuc = await _context.Categories.ToListAsync();
            danhmuc.Insert(0, new Category {  CatId= 0, Catname = "Chọn tất cả" });
            DanhMuc = new SelectList(danhmuc, "CatId", "Catname", catID);
            Categories = new SelectList(await _context.Categories.ToListAsync(), "CatId", "Catname", catID);
            Brands = new SelectList(await _context.Brands.ToListAsync(), "BrandId", "BrandName");
        }
        public JsonResult OnGetFilter(int catID = 0)
        {
            var url = Url.Page("./Index", new { pageIndex = 1, catID });
            return new JsonResult(new { status = "success", targetURL = url });
        }        

    }
}
