using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Helper;
using ProjectThoiTrang.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectThoiTrang.Areas.Admin.Pages.AdminProducts
{
    public class EditModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly INotyfService _notify;
        public EditModel(WebFashionContext context, INotyfService notyf)
        {
            _context = context;
            _notify = notyf;
        }
        [BindProperty]
        public Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products.FindAsync(id);

            if (Product == null)
            {
                return NotFound();
            }

            ViewData["Brand"] = new SelectList(_context.Brands, "BrandId", "BrandName", Product.BrandId);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "Catname", Product.CatId);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id, Microsoft.AspNetCore.Http.IFormFile fthumb)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (id != Product.ProductId)
            {
                return NotFound();
            }

            // _context.Attach(Product).State = EntityState.Modified;

            try
            {
                Product.Productname = Function.Function.toTitleCase(Product.Productname);               
                if (fthumb != null)
                {
                    string extension = Path.GetExtension(fthumb.FileName);
                    string img = Help.SUrl(Product.Productname) + extension;
                    Product.Thumb = await Help.UploadFile(fthumb, @"Product", img.ToLower());
                }
                if (string.IsNullOrEmpty(Product.Thumb)) Product.Thumb = Helper.Help.SUrl(Product.Productname);
                Product.DateModified = DateOnly.FromDateTime(DateTime.Now);
                _context.Update(Product);
                await _context.SaveChangesAsync();
                _notify.Success("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await ProductExists(Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.ProductId == id);
        }

    }
}
