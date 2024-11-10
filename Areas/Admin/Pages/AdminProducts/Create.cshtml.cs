using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectThoiTrang.Helper;
using ProjectThoiTrang.Models;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminProducts
{
    public class CreateModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notify;

        public CreateModel(WebFashionContext context, INotyfService notify)
        {
            _context = context;
            _notify = notify;
        }

        [BindProperty]
        public Product Product { get; set; }
        public SelectList Brands { get; set; }
        public SelectList Categories { get; set; }

        public void OnGet()
        {
            Brands = new SelectList(_context.Brands, "BrandId", "BrandName");
            Categories = new SelectList(_context.Categories, "CatId", "Catname");
        }

        public async Task<IActionResult> OnPostAsync(IFormFile fthumb)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Product.Productname = Function.Function.toTitleCase(Product.Productname);
            if (fthumb != null)
            {
                string extension = Path.GetExtension(fthumb.FileName);
                string img = Help.SUrl(Product.Productname) + extension;
                Product.Thumb = await Help.UploadFile(fthumb, @"Product", img.ToLower());
            }
            if (string.IsNullOrEmpty(Product.Thumb))
            {
                Product.Thumb = "default.png";
            }
            Product.DateModified = DateOnly.FromDateTime(DateTime.Now);
            Product.DateCreated = DateOnly.FromDateTime(DateTime.Now);

            _context.Add(Product);
            await _context.SaveChangesAsync();
            _notify.Success("Tạo mới thành công");
            return RedirectToPage("Index");
        }
    }
}
