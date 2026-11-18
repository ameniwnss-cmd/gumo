using Gumo.Core.Entities;
using Gumo.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Tests;

public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public GumoDbContext Context { get; }

    public TestDb(bool applyMigrations = false)
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<GumoDbContext>().UseSqlite(_connection).Options;
        Context = new GumoDbContext(options);
        if (!applyMigrations)
            Context.Database.EnsureCreated();
    }

    public Location AddLocation(string name = "GuMo Test")
    {
        var location = new Location { Name = name, Address = "Cologne", Hours = "8-18" };
        Context.Locations.Add(location);
        Context.SaveChanges();
        return location;
    }

    public MenuItem AddMenuItem(string name, decimal price, bool available = true)
    {
        var category = Context.Categories.FirstOrDefault() ?? new Category { Name = "Test", DisplayOrder = 1 };
        var item = new MenuItem { Name = name, Price = price, IsAvailable = available, Category = category };
        Context.MenuItems.Add(item);
        Context.SaveChanges();
        return item;
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
