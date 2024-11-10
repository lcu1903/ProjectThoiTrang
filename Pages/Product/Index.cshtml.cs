using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly WebFashionContext _context;                
        public SelectList Brands { get; set; }
        public SelectList Categories { get; set; }
        public string CurrentSort {  get; set; }
        public string CurrentFilter {  get; set; }
        [BindProperty(SupportsGet =true)]
        public string SearchString {  get; set; }
        public int CurrCat {  get; set; }
        public IndexModel(WebFashionContext context)
        {
            _context = context;            
        }
        public PaginatedList<Models.Product> Products { get; set; }
        public async Task OnGetAsync(string sortOption, int? pageIndex,string searchString)
        {
            CurrentSort = sortOption;                                   
            Brands = new SelectList(await _context.Brands.ToListAsync(), "BrandId", "BrandName");
            Categories = new SelectList(await _context.Categories.ToListAsync(), "CatId", "Catname");
            IQueryable<Models.Product> product = from p in _context.Products.Where(p=>p.Active==true) select p ;            
            

            if(!searchString.IsNullOrEmpty())
            {
                product = product.Where(p => p.Productname.Contains(searchString)) ;                
            }
            SearchString = searchString;

            //Sắp xếp
            switch (sortOption)
            {
                case "lowToHigh":
                    product = product.OrderBy(p => p.Price);
                    break;
                case "highToLow":
                    product = product.OrderByDescending(p => p.Price);
                    break;
                default:                    
                    product = product.OrderBy(p => p.Price);
                    break;
            }
            //Phân trang
            var pageSize = Helper.Help.page_size;
            Products = await PaginatedList<Models.Product>.CreateAsync(product.AsNoTracking(), pageIndex ?? 1, pageSize);
            
        }        
    }
}
