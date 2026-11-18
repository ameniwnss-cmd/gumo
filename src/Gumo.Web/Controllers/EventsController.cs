using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Gumo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Web.Controllers;

public class EventsController(GumoDbContext db, EmailService email) : Controller
{
    public async Task<IActionResult> Index()
    {
        var events = await db.Events
            .Include(e => e.Location)
            .Include(e => e.Bookings)
            .Where(e => e.IsPublished && e.Date >= DateTime.UtcNow)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return View(events);
    }

    [HttpGet("Events/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var ev = await db.Events
            .Include(e => e.Location)
            .Include(e => e.Bookings)
            .FirstOrDefaultAsync(e => e.Slug == slug && e.IsPublished);

        if (ev == null) return NotFound();

        return View(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Book([FromBody] BookingRequest request)
    {
        // Load location in the same query — no second round trip needed
        var ev = await db.Events
            .Include(e => e.Location)
            .Include(e => e.Bookings)
            .FirstOrDefaultAsync(e => e.Id == request.EventId && e.IsPublished);

        if (ev == null)
            return BadRequest(new { error = "Event not found." });

        if (ev.AvailableSeats < request.NumberOfSeats)
            return BadRequest(new { error = $"Only {ev.AvailableSeats} seats remaining." });

        var code = "GUMO-" + Guid.NewGuid().ToString("N")[..6].ToUpper();

        var booking = new EventBooking
        {
            EventId = ev.Id,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            NumberOfSeats = request.NumberOfSeats,
            ConfirmationCode = code
        };

        db.EventBookings.Add(booking);
        await db.SaveChangesAsync();

        // Capture plain values before DbContext is disposed — never pass tracked entities to background tasks
        var emailData = new
        {
            CustomerName  = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            NumberOfSeats = booking.NumberOfSeats,
            ConfirmationCode = code,
            EventTitle    = ev.Title,
            EventDate     = ev.Date,
            LocationName  = ev.Location.Name
        };

        _ = Task.Run(async () =>
        {
            try { await email.SendEventBookingConfirmationAsync(emailData.CustomerName, emailData.CustomerEmail, emailData.NumberOfSeats, emailData.ConfirmationCode, emailData.EventTitle, emailData.EventDate, emailData.LocationName); }
            catch (Exception ex) { Console.WriteLine($"[Email] Event booking confirmation failed: {ex.Message}"); }
        });

        return Ok(new { confirmationCode = code, eventTitle = ev.Title, eventDate = ev.Date });
    }
}

public class BookingRequest
{
    public int EventId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int NumberOfSeats { get; set; }
}