using System.Net.Mail;
using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Infrastructure.Services;

public record OrderLine(int MenuItemId, int Quantity);

public record NewOrder(
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone,
    int LocationId,
    string? Notes,
    IReadOnlyList<OrderLine> Lines);

public record CreateOrderResult(Order? Order, string? Error)
{
    public bool Succeeded => Order != null;
}

public class OrderService(GumoDbContext db)
{
    public const int MaxQuantityPerLine = 50;

    public async Task<CreateOrderResult> CreateAsync(NewOrder request)
    {
        if (request.Lines.Count == 0)
            return Fail("Cart is empty.");

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            return Fail("Name is required.");

        if (!MailAddress.TryCreate(request.CustomerEmail, out _))
            return Fail("A valid email address is required.");

        if (request.Lines.Any(l => l.Quantity < 1 || l.Quantity > MaxQuantityPerLine))
            return Fail($"Quantity must be between 1 and {MaxQuantityPerLine}.");

        var location = await db.Locations.FindAsync(request.LocationId);
        if (location == null)
            return Fail("Unknown location.");

        var ids = request.Lines.Select(l => l.MenuItemId).Distinct().ToList();
        var menuItems = await db.MenuItems
            .Where(m => ids.Contains(m.Id) && m.IsAvailable)
            .ToDictionaryAsync(m => m.Id);

        var items = request.Lines
            .Where(l => menuItems.ContainsKey(l.MenuItemId))
            .Select(l => new OrderItem
            {
                MenuItem = menuItems[l.MenuItemId],
                Quantity = l.Quantity,
                UnitPrice = menuItems[l.MenuItemId].Price
            })
            .ToList();

        if (items.Count == 0)
            return Fail("No valid items found.");

        var order = new Order
        {
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone,
            Location = location,
            Notes = request.Notes,
            Items = items
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return new CreateOrderResult(order, null);
    }

    private static CreateOrderResult Fail(string error) => new(null, error);
}
