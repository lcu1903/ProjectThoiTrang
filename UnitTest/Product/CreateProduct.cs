using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectThoiTrang.Areas.Admin.Pages.AdminProducts;
using ProjectThoiTrang.Models;

public class CreateProductTests
{
    private readonly Mock<WebFashionContext> _mockContext;
    private readonly Mock<DbSet<Product>> _mockDbSet;
    private readonly Mock<INotyfService> _mockNotify;
    private readonly CreateModel _createModel;

    public CreateProductTests()
    {
        _mockContext = new Mock<WebFashionContext>();
        _mockDbSet = new Mock<DbSet<Product>>();
        _mockNotify = new Mock<INotyfService>();

        _mockContext.Setup(m => m.Set<Product>()).Returns(_mockDbSet.Object);

        _createModel = new CreateModel(_mockContext.Object, _mockNotify.Object)
        {
            Product = new Product()
        };
    }

    [Fact]
    public async Task OnPostAsync_ModelStateInvalid_ReturnsPage()
    {
        // Arrange
        _createModel.ModelState.AddModelError("Error", "Model state is invalid");

        // Act
        var result = await _createModel.OnPostAsync(null);

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ValidModel_AddProduct()
    {
        // Arrange
        _createModel.Product.Productname = "test product";
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns("test.jpg");

        // Act
        var result = await _createModel.OnPostAsync(formFileMock.Object);

        // Assert
        _mockContext.Verify(m => m.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }

}