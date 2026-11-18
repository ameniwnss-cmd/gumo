using System.Diagnostics;
using Gumo.Infrastructure.Data;
using Gumo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class HomeController(GumoDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var upcomingEvents = await db.Events
            .Include(e => e.Location)
            .Include(e => e.Bookings)
            .Where(e => e.IsPublished && e.Date >= DateTime.UtcNow)
            .OrderBy(e => e.Date)
            .Take(3)
            .ToListAsync();

        return View(upcomingEvents);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}