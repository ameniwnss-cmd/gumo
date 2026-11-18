namespace Gumo.Core.Entities;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Hours { get; set; } = string.Empty;
    public string? GoogleMapsUrl { get; set; }
    public string? Phone { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Event> Events { get; set; } = [];
}
