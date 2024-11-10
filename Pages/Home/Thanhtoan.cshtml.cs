using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.RequestModel;
using ProjectThoiTrang.Service;

namespace ProjectThoiTrang.Pages.Home
{
    public class ThanhtoanModel : PageModel
    {
        private readonly WebFashionContext _context;
        private readonly IVnPayService _vnPayService;
        public ThanhtoanModel (WebFashionContext context,IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }
             
    }
}
