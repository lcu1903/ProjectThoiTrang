using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectThoiTrang.Areas.Admin.Pages.AdminProducts;
using ProjectThoiTrang.Models;
using System.Threading.Tasks;
using Xunit;

public class EditProduct
{
    private readonly Mock<WebFashionContext> _mockContext;
    private readonly Mock<DbSet<Product>> _mockDbSet;
    private readonly Mock<INotyfService> _mockNotify;
    private readonly EditModel _editModel;

    public EditProduct()
    {
        _mockContext = new Mock<WebFashionContext>();
        _mockDbSet = new Mock<DbSet<Product>>();
        _mockNotify = new Mock<INotyfService>();

        _mockContext.Setup(m => m.Set<Product>()).Returns(_mockDbSet.Object);

        _editModel = new EditModel(_mockContext.Object, _mockNotify.Object)
        {
            Product = new Product()
        };
    }

    [Fact]
    public async Task OnPostAsync_ModelStateInvalid_ReturnsPage()
    {
        // Arrange
        _editModel.ModelState.AddModelError("Error", "Model state is invalid");

        // Act
        var result = await _editModel.OnPostAsync(1, null);

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_IdMismatch_ReturnsNotFound()
    {
        // Arrange
        _editModel.Product.ProductId = 2;

        // Act
        var result = await _editModel.OnPostAsync(1, null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ValidModel_UpdatesProduct()
    {
        // Arrange
        _editModel.Product.ProductId = 1;
        _editModel.Product.Productname = "test product";
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns("test.jpg");

        // Act
        var result = await _editModel.OnPostAsync(1, formFileMock.Object);

        // Assert
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }


    [Fact]
    public async Task OnPostAsync_NullReferenceException_ReturnsNotFound()
    {
        // Arrange
        _editModel.Product.ProductId = 2;

        // Act
        var result = await _editModel.OnPostAsync(1, null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}