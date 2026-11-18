using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class OrderTrackingController(GumoDbContext db) : Controller
{
    public async Task<IActionResult> Index(string token)
    {
        var order = await db.Orders
            .Include(o => o.Location)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.TrackingToken == token);

        if (order == null) return NotFound();

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Status(string token)
    {
        var order = await db.Orders
            .Where(o => o.TrackingToken == token)
            .Select(o => new { o.Status })
            .FirstOrDefaultAsync();

        if (order == null) return NotFound();

        return Ok(new { status = order.Status.ToString() });
    }
}