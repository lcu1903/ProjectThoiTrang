using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectThoiTrang.Areas.Admin.Pages.AdminCategories;
using ProjectThoiTrang.Models;
using System.Threading.Tasks;
using Xunit;

public class EditCategory
{
    private readonly Mock<WebFashionContext> _mockContext;
    private readonly Mock<DbSet<Category>> _mockDbSet;
    private readonly Mock<INotyfService> _mockNotify;
    private readonly CreateModel _createModel;
    private readonly EditModel _editModel;

    public EditCategory()
    {
        _mockContext = new Mock<WebFashionContext>();
        _mockDbSet = new Mock<DbSet<Category>>();
        _mockNotify = new Mock<INotyfService>();

        _mockContext.Setup(m => m.Set<Category>()).Returns(_mockDbSet.Object);

        _createModel = new CreateModel(_mockContext.Object, _mockNotify.Object);

        _editModel = new EditModel(_mockContext.Object, _mockNotify.Object)
        {
            Category = new Category { CatId = 1, Catname = "Test Category" }
        }; ;
    }

    [Fact]
    public async Task CreateOnPostAsync_ModelStateInvalid_ReturnsPage()
    {
        // Arrange
        _createModel.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _createModel.OnPostAsync();

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
    }
    [Fact]
    public async Task CreateOnPostAsync_ModelStateValid_AddsCategoryAndRedirects()
    {
        // Arrange
        _createModel.ModelState.Clear();

        // Act
        var result = await _createModel.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_ModelStateInvalid_ReturnsPage()
    {
        // Arrange
        _editModel.ModelState.AddModelError("Error", "Model state is invalid");

        // Act
        var result = await _editModel.OnPostAsync(1);

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_IdMismatch_ReturnsNotFound()
    {
        // Arrange
        _editModel.Category.CatId = 2;

        // Act
        var result = await _editModel.OnPostAsync(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ValidModel_UpdatesCategoryAndRedirects()
    {
        // Arrange
        _editModel.Category.CatId = 1;
        _editModel.Category.Catname = "test cat";
        // Act
        var result = await _editModel.OnPostAsync(1);

        // Assert
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        Assert.IsType<RedirectToPageResult>(result);
    }
}