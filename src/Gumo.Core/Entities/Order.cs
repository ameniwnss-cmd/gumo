namespace Gumo.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string TrackingToken { get; set; } = Guid.NewGuid().ToString("N")[..12];

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = [];

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Ready,
    PickedUp,
    Cancelled
}
