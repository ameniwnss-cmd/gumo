using Gumo.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gumo.Infrastructure.Data;

public class GumoDbContext(DbContextOptions<GumoDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventBooking> EventBookings => Set<EventBooking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MenuItem>()
            .Property(m => m.Price)
            .HasColumnType("decimal(10,2)");

        builder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasColumnType("decimal(10,2)");
    }
}
