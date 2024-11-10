using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectThoiTrang.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ProjectThoiTrang.Pages.Account
{    
    public class RegisterModel : PageModel
    {
        private readonly WebFashionContext _context;
        [BindProperty]
        public NewAccount newAccount { get; set; }
        public RegisterModel(WebFashionContext context)
        {
            _context = context;
        }
        public class NewAccount
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            public string FullName { get; set; }
            [Required]
            public string PhoneNumber { get; set; }
            [Required]
            public DateOnly Birthday { get; set; }
            [Required]
            public string Address {  get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var existingAccount =  _context.Accounts.FirstOrDefault(a => a.Email == newAccount.Email);
            if (existingAccount != null) {                
                return Page();
            }
            int newId;
            do
            {
                newId = Helper.Help.GenerateID();
            } while (_context.Accounts.Any(a => a.AccountId == newId));
            //tạo tài khoản mới
            var account = new Models.Account
            {
                AccountId = newId,
                Email = newAccount.Email,
                Password = newAccount.Password,
                RoleId = 2,
                Active = true,
                CreateDate = DateOnly.FromDateTime(DateTime.Now),                
            };

            _context.Accounts.Add(account);
            // tạo 1 khách hàng tương ứng với email
            var customer = new Customer
            {        
                CusId = account.AccountId,
                Email = newAccount.Email,       
                Fullname = newAccount.FullName,
                Birthday = newAccount.Birthday,
                Address = newAccount.Address,
                Phone = newAccount.PhoneNumber,
                Active = true,
                CreateDate = DateOnly.FromDateTime(DateTime.Now),                
            };

            _context.Customers.Add(customer);
            var cart = new Models.Cart
            {
                CusId = customer.CusId,
                Paid = false,
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Login"); 
        }

    }
}
