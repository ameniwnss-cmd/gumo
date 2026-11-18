using Gumo.Core.Entities;
using Gumo.Infrastructure.Services;

namespace Gumo.Tests;

public class OrderServiceTests : IDisposable
{
    private readonly TestDb _db = new();
    private readonly OrderService _service;

    public OrderServiceTests() => _service = new OrderService(_db.Context);

    public void Dispose() => _db.Dispose();

    private static NewOrder Request(int locationId, params OrderLine[] lines) =>
        new("Ada Lovelace", "ada@example.com", null, locationId, null, lines);

    [Fact]
    public async Task Creates_order_with_prices_taken_from_the_database()
    {
        var location = _db.AddLocation();
        var latte = _db.AddMenuItem("Latte", 4.00m);
        var bagel = _db.AddMenuItem("Bagel", 7.50m);

        var result = await _service.CreateAsync(
            Request(location.Id, new OrderLine(latte.Id, 2), new OrderLine(bagel.Id, 1)));

        Assert.True(result.Succeeded);
        Assert.Equal(15.50m, result.Order!.Total);
        Assert.Equal(OrderStatus.Pending, result.Order.Status);
        Assert.Equal(2, _db.Context.OrderItems.Count());
    }

    [Fact]
    public async Task Generates_a_12_character_tracking_token()
    {
        var location = _db.AddLocation();
        var latte = _db.AddMenuItem("Latte", 4.00m);

        var result = await _service.CreateAsync(Request(location.Id, new OrderLine(latte.Id, 1)));

        Assert.Equal(12, result.Order!.TrackingToken.Length);
    }

    [Fact]
    public async Task Tracking_tokens_are_unique_per_order()
    {
        var location = _db.AddLocation();
        var latte = _db.AddMenuItem("Latte", 4.00m);

        var first = await _service.CreateAsync(Request(location.Id, new OrderLine(latte.Id, 1)));
        var second = await _service.CreateAsync(Request(location.Id, new OrderLine(latte.Id, 1)));

        Assert.NotEqual(first.Order!.TrackingToken, second.Order!.TrackingToken);
    }

    [Fact]
    public async Task Rejects_an_empty_cart()
    {
        var location = _db.AddLocation();

        var result = await _service.CreateAsync(Request(location.Id));

        Assert.False(result.Succeeded);
        Assert.Equal("Cart is empty.", result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(OrderService.MaxQuantityPerLine + 1)]
    public async Task Rejects_out_of_range_quantities(int quantity)
    {
        var location = _db.AddLocation();
        var latte = _db.AddMenuItem("Latte", 4.00m);

        var result = await _service.CreateAsync(Request(location.Id, new OrderLine(latte.Id, quantity)));

        Assert.False(result.Succeeded);
        Assert.Empty(_db.Context.Orders);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Rejects_invalid_email(string email)
    {
        var location = _db.AddLocation();
        var latte = _db.AddMenuItem("Latte", 4.00m);
        var request = new NewOrder("Ada", email, null, location.Id, null, [new OrderLine(latte.Id, 1)]);

        var result = await _service.CreateAsync(request);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Rejects_unknown_location()
    {
        var latte = _db.AddMenuItem("Latte", 4.00m);

        var result = await _service.CreateAsync(Request(999, new OrderLine(latte.Id, 1)));

        Assert.False(result.Succeeded);
        Assert.Equal("Unknown location.", result.Error);
    }

    [Fact]
    public async Task Ignores_unavailable_items_and_fails_when_none_remain()
    {
        var location = _db.AddLocation();
        var soldOut = _db.AddMenuItem("Sold out", 5.00m, available: false);
        var latte = _db.AddMenuItem("Latte", 4.00m);

        var mixed = await _service.CreateAsync(
            Request(location.Id, new OrderLine(soldOut.Id, 1), new OrderLine(latte.Id, 1)));
        var onlySoldOut = await _service.CreateAsync(Request(location.Id, new OrderLine(soldOut.Id, 1)));

        Assert.Single(mixed.Order!.Items);
        Assert.False(onlySoldOut.Succeeded);
    }
}
