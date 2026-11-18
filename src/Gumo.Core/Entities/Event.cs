namespace Gumo.Core.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public string? ImageFileName { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<EventBooking> Bookings { get; set; } = [];

    public int BookedSeats => Bookings.Sum(b => b.NumberOfSeats);
    public int AvailableSeats => Capacity - BookedSeats;
}
