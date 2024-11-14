using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Service;

namespace ProjectThoiTrang.UnitTest.Purchase
{
    public class GetCartDetailsTest
    {
        private readonly Mock<WebFashionContext> _mockContext;
        private readonly ServiceCart _cart;
        private readonly Mock<DbSet<Cart>> _mockDbSet;

        public GetCartDetailsTest()
        {
            _mockContext = new Mock<WebFashionContext>();
            _cart = new ServiceCart(_mockContext.Object);
            _mockDbSet = new Mock<DbSet<Cart>>();
        }

        [Fact]
        public async Task GetCartDeTails_CartNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockContext.Setup(m => m.Set<Cart>()).Returns(_mockDbSet.Object);

            // Act
            var result = await _cart.GetCartDeTails(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Cart not found or no details available.", ((dynamic)notFoundResult.Value).Message);
        }

        [Fact]
        public async Task GetCartDeTails_CartExists_ReturnsCartDetailsCount()
        {
            // Arrange
            var userId = 1;
            var cart = new Cart
            {
                CusId = userId,
                Paid = false,
                CartDetails = new List<CartDetail>
            {
                new CartDetail { Product = new Product() },
                new CartDetail { Product = new Product() }
            }
            };
            _mockContext.Setup(m => m.Set<Cart>()).Returns(_mockDbSet.Object);// Không có giỏ hàng cho userId

            // Act
            var result = await _cart.GetCartDeTails(userId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True(((dynamic)jsonResult.Value).success);
            Assert.Equal(2, ((dynamic)jsonResult.Value).cartamout);
        }
    }
}
