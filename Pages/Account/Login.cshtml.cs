using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using System.Security.Claims;

namespace ProjectThoiTrang.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly WebFashionContext _context;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(WebFashionContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = await _context.Accounts
                                       .FirstOrDefaultAsync(a => a.Email == Email && a.Password == Password && a.Active == true);            
            
            if (account != null)
            {
                if (account.RoleId == 2)
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Email == account.Email);
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString()),
                    new Claim("AccountId", account.AccountId.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToPage("/Index");
                }
                else
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Email == account.Email);
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString()),
                    new Claim("AccountId", account.AccountId.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToPage("/AdminHome/Index", new { area = "Admin" });
                }

            }            
            return RedirectToPage("/Account/CanhBao");
        }
    }
}
