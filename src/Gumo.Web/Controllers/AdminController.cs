using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

[Authorize]
public class AdminController(GumoDbContext db, IWebHostEnvironment env) : Controller
{
    // ── Image upload ─────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file, string folder = "menu")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            return BadRequest(new { error = "Only JPG, PNG or WebP images are allowed." });

        var uploadFolder = Path.Combine(env.WebRootPath, "img", folder);
        Directory.CreateDirectory(uploadFolder);

        var fileName = Guid.NewGuid().ToString("N") + ext;
        var filePath = Path.Combine(uploadFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { fileName });
    }

    // ── Orders dashboard ────────────────────────────────────────────────────

    public async Task<IActionResult> Orders(int? locationId)
    {
        var locations = await db.Locations.ToListAsync();
        ViewBag.Locations = locations;
        ViewBag.SelectedLocationId = locationId;

        var query = db.Orders
            .Include(o => o.Location)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.Status != OrderStatus.PickedUp && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.CreatedAt)
            .AsQueryable();

        if (locationId.HasValue)
            query = query.Where(o => o.LocationId == locationId.Value);

        return View(await query.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = await db.Orders.FindAsync(orderId);
        if (order == null) return NotFound();

        order.Status = status;
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> PollOrders(int? locationId)
    {
        var query = db.Orders
            .Include(o => o.Location)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.Status != OrderStatus.PickedUp && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.CreatedAt)
            .AsQueryable();

        if (locationId.HasValue)
            query = query.Where(o => o.LocationId == locationId.Value);

        var orders = await query.Select(o => new
        {
            o.Id,
            o.CustomerName,
            o.CustomerEmail,
            o.CustomerPhone,
            Status = o.Status.ToString(),
            Location = o.Location.Name,
            o.LocationId,
            CreatedAt = o.CreatedAt.ToString("HH:mm"),
            o.Notes,
            Items = o.Items.Select(i => new
            {
                i.MenuItem.Name,
                i.Quantity,
                i.UnitPrice
            }),
            Total = o.Items.Sum(i => i.UnitPrice * i.Quantity)
        }).ToListAsync();

        return Ok(orders);
    }

    // ── Menu management ─────────────────────────────────────────────────────

    public async Task<IActionResult> Menu()
    {
        var categories = await db.Categories
            .Include(c => c.MenuItems.OrderBy(m => m.DisplayOrder))
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleMenuItem(int id)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item == null) return NotFound();

        item.IsAvailable = !item.IsAvailable;
        await db.SaveChangesAsync();

        return Ok(new { isAvailable = item.IsAvailable });
    }

    [HttpPost]
    public async Task<IActionResult> SaveMenuItem([FromBody] MenuItemSaveRequest request)
    {
        MenuItem item;
        if (request.Id == 0)
        {
            item = new MenuItem { CategoryId = request.CategoryId };
            db.MenuItems.Add(item);
        }
        else
        {
            item = await db.MenuItems.FindAsync(request.Id) ?? new MenuItem();
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.IsVegan = request.IsVegan;
        item.IsAvailable = request.IsAvailable;
        if (!string.IsNullOrEmpty(request.ImageFileName))
            item.ImageFileName = request.ImageFileName;

        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item == null) return NotFound();

        db.MenuItems.Remove(item);
        await db.SaveChangesAsync();
        return Ok();
    }

    // ── Events management ───────────────────────────────────────────────────

    public async Task<IActionResult> Events()
    {
        var events = await db.Events
            .Include(e => e.Location)
            .Include(e => e.Bookings)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        ViewBag.Locations = await db.Locations.ToListAsync();
        return View(events);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEvent([FromBody] EventSaveRequest request)
    {
        var eventDate = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);
        if (eventDate < DateTime.UtcNow)
            return BadRequest(new { error = "Event date cannot be in the past." });

        Event ev;
        if (request.Id == 0)
        {
            ev = new Event();
            db.Events.Add(ev);
        }
        else
        {
            ev = await db.Events.FindAsync(request.Id) ?? new Event();
        }

        ev.Title = request.Title;
        ev.Description = request.Description;
        ev.Date = request.Date;
        ev.Capacity = request.Capacity;
        ev.LocationId = request.LocationId;
        ev.IsPublished = request.IsPublished;
        ev.Slug = request.Title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            + "-" + request.Date.ToString("yyyyMMdd");
        if (!string.IsNullOrEmpty(request.ImageFileName))
            ev.ImageFileName = request.ImageFileName;

        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await db.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null) return NotFound();

        db.Events.Remove(ev);
        await db.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> EventBookings(int eventId)
    {
        var ev = await db.Events
            .Include(e => e.Bookings)
            .Include(e => e.Location)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null) return NotFound();
        return View(ev);
    }
}

public class MenuItemSaveRequest
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsVegan { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageFileName { get; set; }
}

public class EventSaveRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public int LocationId { get; set; }
    public bool IsPublished { get; set; }
    public string? ImageFileName { get; set; }
}