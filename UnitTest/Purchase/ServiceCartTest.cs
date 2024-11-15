using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.Service;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

public class ServiceCartTest
{
    private readonly Mock<WebFashionContext> _contextMock;
    private readonly ServiceCart _serviceCart;

    public ServiceCartTest()
    {
        _contextMock = new Mock<WebFashionContext>();
        _serviceCart = new ServiceCart(_contextMock.Object);
    }

    private class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new AsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    private class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        { }

        public AsyncEnumerable(Expression expression) : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
    }

    private class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }

    private Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> data) where T : class
    {
        var dbSetMock = new Mock<DbSet<T>>();
        dbSetMock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));

        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new AsyncQueryProvider<T>(data.Provider));
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return dbSetMock;
    }

    [Fact]
    public async Task AddToCart_ShouldAddProductToCart()
    {
        // Arrange
        var userId = 1;
        var productId = 1;
        var quantity = 0;

        var cart = new Cart { CusId = userId, Paid = false, CartDetails = new List<CartDetail>() };
        var product = new Product { ProductId = productId, Price = 100, Discount = 10, Stock = 10 };

        var carts = new List<Cart> { cart }.AsQueryable();
        var products = new List<Product> { product }.AsQueryable();

        var cartsDbSetMock = CreateDbSetMock(carts);
        var productsDbSetMock = CreateDbSetMock(products);

        _contextMock.Setup(c => c.Carts).Returns(cartsDbSetMock.Object);
        _contextMock.Setup(c => c.Products).Returns(productsDbSetMock.Object);

        // Act
        var result = await _serviceCart.AddToCart(userId, productId, quantity);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var success = Assert.IsType<bool>(jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value));
        Assert.True(success);
    }
    [Fact]
    public async Task AddToCart_WhenCartDetail_NotNull_And_OrderDetailQuantity_HigherThan_ProductStock()
    {
        // Arrange
        var userId = 1;
        var productId = 1;
        var quantity = 2;

        var cart = new Cart { CusId = userId, Paid = false, CartDetails = new List<CartDetail>() { 
            new CartDetail { 
                CartId = 1,
                Quantity = 3,
                ProductId = productId
            }} 
        };
        var product = new Product { ProductId = productId, Price = 100, Discount = 10, Stock = 4 };

        var carts = new List<Cart> { cart }.AsQueryable();
        var products = new List<Product> { product }.AsQueryable();

        var cartsDbSetMock = CreateDbSetMock(carts);
        var productsDbSetMock = CreateDbSetMock(products);

        _contextMock.Setup(c => c.Carts).Returns(cartsDbSetMock.Object);
        _contextMock.Setup(c => c.Products).Returns(productsDbSetMock.Object);

        // Act
        var result = await _serviceCart.AddToCart(userId, productId, quantity);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var success = Assert.IsType<bool>(jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value));
        Assert.False(success);
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveProductFromCart()
    {
        // Arrange
        var userId = 1;
        var productId = 1;
        var cartDetail = new CartDetail { ProductId = productId, Quantity = 2 };
        var cart = new Cart { CusId = userId, Paid = false, CartDetails = new List<CartDetail> { cartDetail } };

        var carts = new List<Cart> { cart }.AsQueryable();

        var cartsDbSetMock = CreateDbSetMock(carts);

        _contextMock.Setup(c => c.Carts).Returns(cartsDbSetMock.Object);

        // Act
        var result = await _serviceCart.DeleteProduct(userId, productId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var success = Assert.IsType<bool>(jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value));
        Assert.True(success);
    }
}