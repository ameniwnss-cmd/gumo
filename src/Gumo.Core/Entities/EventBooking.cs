namespace Gumo.Core.Entities;

public class EventBooking
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int NumberOfSeats { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public string ConfirmationCode { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
}
