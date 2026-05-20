using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Bookstore.Tests;

public class CheckoutServiceTests
{
    [Fact]
    public async Task CalculatesTax()
    {
        // Arrange
        const string cartId = "cart-abc";
        const decimal bookPrice = 100m;

        // Correct .NET 8 IHttpContextAccessor mock setup:
        // Create a DefaultHttpContext and set the Cookie header so Request.Cookies is populated.
        // The old System.Web.HttpContext.Current pattern does not exist in .NET 8.
        var mockAccessor = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = $"ShoppingCartId={cartId}";
        mockAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var cart = new ShoppingCart(cartId);
        var item = new ShoppingCartItem(cart, 1, 1, wantToBuy: true);
        item.Book = new Book("Test Book", "Author", "ISBN-001", 1, 1, 1, 1, bookPrice, quantity: 5);
        cart.ShoppingCartItems.Add(item);

        var mockCartService = new Mock<IShoppingCartService>();
        mockCartService.Setup(x => x.GetShoppingCartAsync(cartId)).ReturnsAsync(cart);

        var service = new CheckoutService(mockAccessor.Object, mockCartService.Object);

        // Act
        var tax = await service.CalculateTaxAsync();

        // Assert
        Assert.Equal(bookPrice * 0.1m, tax);
    }
}
