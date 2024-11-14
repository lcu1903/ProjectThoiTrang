using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Service;

using Xunit;

namespace ProjectThoiTrang.UnitTest.Purchase
{
    public class DeleteProductCartTest
    {
        private readonly Mock<WebFashionContext> _mockContext;
        private readonly ServiceCart _cart;
        private readonly Mock<DbSet<Cart>> _mockDbSet;

        public DeleteProductCartTest()
        {
            _mockContext = new Mock<WebFashionContext>();
            _cart = new ServiceCart(_mockContext.Object);
            _mockDbSet = new Mock<DbSet<Cart>>();
        }
        [Fact]
        public async Task DeleteProduct_ProductNotFoundInCart_ReturnsSuccessFalse()
        {
            // Arrange
            int userId = 1;
            int productId = 999; // Non-existing product ID
            var cart = new Cart
            {
                CusId = userId,
                Paid = false,
                CartDetails = new List<CartDetail>()
            };

            _mockContext.Setup(m => m.Set<Cart>()).Returns(_mockDbSet.Object);

            // Act
            var result = await _cart.DeleteProduct(userId, productId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False(((dynamic)jsonResult.Value).success);
        }

        [Fact]
        public async Task DeleteProduct_ProductExistsInCart_ReturnsSuccessTrue()
        {
            // Arrange
            int userId = 1;
            int productId = 2;
            var cart = new Cart
            {
                CusId = userId,
                Paid = false,
                CartDetails = new List<CartDetail>
            {
                new CartDetail { ProductId = productId } // Product to be deleted
            }
            };

            _mockContext.Setup(m => m.Set<Cart>()).Returns(_mockDbSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1); // Simulate successful save

            // Act
            var result = await _cart.DeleteProduct(userId, productId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True(((dynamic)jsonResult.Value).success);
            Assert.Empty(cart.CartDetails); // Ensure product is removed from cart
        }
    }
}

