using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Service;
using Xunit;

namespace ProjectThoiTrang.UnitTest.Purchase
{
    public class AddToCartTests
    {
        private WebFashionContext _context;
        private ServiceCart _cart;

        private void InitializeDatabase()
        {
            // Mỗi kiểm thử có một database mới
            var options = new DbContextOptionsBuilder<WebFashionContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new WebFashionContext(options);
            _cart = new ServiceCart(_context);

            // Thêm dữ liệu mẫu vào database
            _context.Products.Add(new Product { ProductId = 1, Productname = "Sample Product", Price = 100, Stock = 5 });
            _context.Products.Add(new Product { ProductId = 2, Productname = "Limited Stock Product", Price = 200, Stock = 2 });
            _context.Customers.Add(new Customer { CusId = 1, Fullname = "Sample Customer" });
            _context.SaveChanges();
        }
        [Fact]
        public async Task AddToCart_NewCart_Success()
        {
            InitializeDatabase();

            // Kiểm tra thêm sản phẩm vào giỏ hàng khi giỏ hàng chưa tồn tại
            var result = await _cart.AddToCart(1, 1, 2); // Thêm sản phẩm với số lượng hợp lệ
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = jsonResult.Value.GetType().GetProperty("success")?.GetValue(jsonResult.Value, null);
            Assert.True((bool)success);

            // Xác minh rằng giỏ hàng đã được tạo và có chi tiết giỏ hàng đúng
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.CusId == 1 && c.Paid == false);
            Assert.NotNull(cart);
            Assert.Single(cart.CartDetails);
        }

        [Fact]
        public async Task AddToCart_ExistingCart_AddProductQuantity()
        {
            InitializeDatabase();

            // Khởi tạo giỏ hàng và thêm sản phẩm vào giỏ hàng trước đó
            await _cart.AddToCart(1, 1, 2);

            // Thêm cùng sản phẩm với số lượng bổ sung
            var result = await _cart.AddToCart(1, 1, 2); // Thêm số lượng thêm cho sản phẩm đã có trong giỏ
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = jsonResult.Value.GetType().GetProperty("success")?.GetValue(jsonResult.Value, null);
            Assert.True((bool)success);

            // Kiểm tra số lượng đã cập nhật
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.CusId == 1 && c.Paid == false);
            Assert.NotNull(cart);
            Assert.Single(cart.CartDetails);
        }

        [Fact]
        public async Task AddToCart_ExceedStock_LimitQuantity()
        {
            InitializeDatabase();

            // Thêm sản phẩm vượt quá tồn kho
            var result = await _cart.AddToCart(1, 2, 5); // Sản phẩm 2 chỉ có 2 tồn kho
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = jsonResult.Value.GetType().GetProperty("success")?.GetValue(jsonResult.Value, null);
            Assert.True((bool)success);

            // Kiểm tra số lượng đã được giới hạn ở tồn kho
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.CusId == 1 && c.Paid == false);
            Assert.NotNull(cart);
            Assert.Single(cart.CartDetails);
        }

        [Fact]
        public async Task AddToCart_ProductNotAvailable_Fail()
        {
            InitializeDatabase();

            // Thử thêm sản phẩm không tồn tại
            var result = await _cart.AddToCart(1, 999, 1); // Sản phẩm ID 999 không tồn tại
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = jsonResult.Value.GetType().GetProperty("success")?.GetValue(jsonResult.Value, null);
            Assert.False((bool)success);
        }
    }
}
