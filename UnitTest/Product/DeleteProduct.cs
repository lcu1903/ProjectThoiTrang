using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Areas.Admin.Pages.AdminProducts;
using ProjectThoiTrang.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Threading.Tasks;

public class DeleteModelTests
{
    private readonly Mock<WebFashionContext> _contextMock;
    private readonly Mock<INotyfService> _notifyMock;
    private readonly DeleteModel _deleteModel;

    public DeleteModelTests()
    {
        _contextMock = new Mock<WebFashionContext>();
        _notifyMock = new Mock<INotyfService>();
        _deleteModel = new DeleteModel(_contextMock.Object, _notifyMock.Object);
    }

    [Fact]
    public async Task OnPost_IdIsNull_ReturnsNotFound()
    {
        // Act
        var result = await _deleteModel.OnPost(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPost_ProductFound_DeletesProductAndReturnsRedirect()
    {
        // Arrange
        var product = new Product { ProductId = 1 };
        _contextMock.Setup(c => c.Products.FindAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _deleteModel.OnPost(1);

        // Assert
        _contextMock.Verify(c => c.Products.Remove(product), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }

    [Fact]
    public async Task OnPost_ProductNotFound_ReturnsRedirect()
    {
        // Arrange
        _contextMock.Setup(c => c.Products.FindAsync(1)).ReturnsAsync((Product)null);

        // Act
        var result = await _deleteModel.OnPost(1);

        // Assert
        _contextMock.Verify(c => c.Products.Remove(It.IsAny<Product>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }
}