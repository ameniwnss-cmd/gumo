using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Gumo.Web.Controllers;

public class CheckInController(GumoDbContext db, IMemoryCache cache) : Controller
{
    // ── Admin generates a one-time session link ──────────────────────────────

    [Authorize]
    [HttpPost]
    public IActionResult GenerateSession()
    {
        var token = Guid.NewGuid().ToString("N")[..16];
        // Expires after 90s if no heartbeat received
        cache.Set(CacheKey(token), true, TimeSpan.FromSeconds(90));
        var url = $"{Request.Scheme}://{Request.Host}/CheckIn/{token}";
        return Ok(new { token, url });
    }

    [Authorize]
    [HttpPost]
    public IActionResult RevokeSession([FromBody] TokenRequest request)
    {
        cache.Remove(CacheKey(request.Token));
        return Ok();
    }

    // ── Staff opens the link ─────────────────────────────────────────────────

    [HttpGet("CheckIn/{token}")]
    public IActionResult Index(string token)
    {
        if (!cache.TryGetValue(CacheKey(token), out _))
            return View("Expired");

        ViewBag.Token = token;
        return View();
    }

    // ── Heartbeat — keeps session alive while tab is open ────────────────────

    [HttpPost]
    public IActionResult Heartbeat([FromBody] TokenRequest request)
    {
        if (!cache.TryGetValue(CacheKey(request.Token), out _))
            return Ok(new { alive = false });

        // Refresh TTL — 90 seconds from now
        cache.Set(CacheKey(request.Token), true, TimeSpan.FromSeconds(90));
        return Ok(new { alive = true });
    }

    // ── QR code verification ─────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
    {
        if (!cache.TryGetValue(CacheKey(request.Token), out _))
            return Unauthorized(new { error = "Session expired. Ask admin for a new link." });

        var booking = await db.EventBookings
            .Include(b => b.Event).ThenInclude(e => e.Location)
            .FirstOrDefaultAsync(b => b.ConfirmationCode == request.Code);

        if (booking == null)
            return Ok(new { result = "invalid" });

        if (booking.IsUsed)
            return Ok(new
            {
                result = "used",
                usedAt = booking.UsedAt!.Value.ToLocalTime().ToString("HH:mm")
            });

        booking.IsUsed = true;
        booking.UsedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(new
        {
            result = "valid",
            customerName = booking.CustomerName,
            numberOfSeats = booking.NumberOfSeats,
            eventTitle = booking.Event.Title,
            eventDate = booking.Event.Date.ToString("MMM d, yyyy · HH:mm"),
            location = booking.Event.Location.Name
        });
    }

    private static string CacheKey(string token) => $"checkin:{token}";
}

public class VerifyRequest
{
    public string Code { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class TokenRequest
{
    public string Token { get; set; } = string.Empty;
}