using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class LocationsController(GumoDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var locations = await db.Locations.ToListAsync();
        return View(locations);
    }
}