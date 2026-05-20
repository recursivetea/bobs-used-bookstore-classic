using Bookstore.Domain.Carts;
using Bookstore.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace Bookstore.Web.Services;

public class CheckoutService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IShoppingCartService _shoppingCartService;

    public CheckoutService(IHttpContextAccessor httpContextAccessor, IShoppingCartService shoppingCartService)
    {
        _httpContextAccessor = httpContextAccessor;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<decimal> CalculateTaxAsync()
    {
        var correlationId = _httpContextAccessor.HttpContext!.GetShoppingCartCorrelationId();
        var cart = await _shoppingCartService.GetShoppingCartAsync(correlationId);
        return cart.GetSubTotal(ShoppingCartItemFilter.ExcludeOutOfStockItems) * 0.1m;
    }
}
