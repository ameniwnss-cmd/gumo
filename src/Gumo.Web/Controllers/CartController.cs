using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Gumo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class CartController(GumoDbContext db, OrderService orders, EmailService email) : Controller
{
    public async Task<IActionResult> Index()
    {
        var locations = await db.Locations.ToListAsync();
        ViewBag.Locations = locations;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var result = await orders.CreateAsync(new NewOrder(
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.LocationId,
            request.Notes,
            (request.Items ?? []).Select(i => new OrderLine(i.MenuItemId, i.Quantity)).ToList()));

        if (!result.Succeeded)
            return BadRequest(new { error = result.Error });

        var order = result.Order!;

        // Capture plain values — never pass tracked EF entities into background tasks
        var trackingUrl = $"{Request.Scheme}://{Request.Host}/order/track/{order.TrackingToken}";
        var emailItems = order.Items.Select(i => (i.MenuItem.Name, i.Quantity, i.UnitPrice)).ToList();
        var orderId = order.Id;
        var customerName = order.CustomerName;
        var customerEmail = order.CustomerEmail;
        var locationName = order.Location.Name;

        _ = Task.Run(async () =>
        {
            try { await email.SendOrderConfirmationAsync(orderId, customerName, customerEmail, locationName, emailItems, trackingUrl); }
            catch (Exception ex) { Console.WriteLine($"[Email] Order confirmation failed: {ex.Message}"); }
        });

        return Ok(new { trackingToken = order.TrackingToken });
    }
}


public class CheckoutRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int LocationId { get; set; }
    public string? Notes { get; set; }
    public List<CartItemRequest> Items { get; set; } = [];
}

public class CartItemRequest
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}