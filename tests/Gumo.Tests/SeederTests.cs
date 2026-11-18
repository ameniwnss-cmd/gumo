using Gumo.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gumo.Tests;

public class SeederTests : IDisposable
{
    private readonly TestDb _db = new(applyMigrations: true);
    private readonly ServiceProvider _services;

    public SeederTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_db.Context);
        services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<GumoDbContext>();
        _services = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _services.Dispose();
        _db.Dispose();
    }

    private UserManager<IdentityUser> Users => _services.GetRequiredService<UserManager<IdentityUser>>();

    [Fact]
    public async Task Seeds_locations_and_menu()
    {
        await DbSeeder.SeedAsync(_db.Context, Users);

        Assert.Equal(2, _db.Context.Locations.Count());
        Assert.NotEmpty(_db.Context.MenuItems);
        Assert.All(_db.Context.MenuItems, m => Assert.True(m.Price > 0));
    }

    [Fact]
    public async Task Does_not_create_an_admin_without_configured_credentials()
    {
        await DbSeeder.SeedAsync(_db.Context, Users);

        Assert.Empty(Users.Users);
    }

    [Fact]
    public async Task Creates_the_admin_from_configured_credentials()
    {
        await DbSeeder.SeedAsync(_db.Context, Users, "owner@example.com", "Str0ng-Passw0rd!");

        var admin = Assert.Single(Users.Users);
        Assert.Equal("owner@example.com", admin.Email);
        Assert.True(await Users.CheckPasswordAsync(admin, "Str0ng-Passw0rd!"));
    }

    [Fact]
    public async Task Seeding_twice_is_idempotent()
    {
        await DbSeeder.SeedAsync(_db.Context, Users, "owner@example.com", "Str0ng-Passw0rd!");
        var menuCount = _db.Context.MenuItems.Count();

        await DbSeeder.SeedAsync(_db.Context, Users, "owner@example.com", "Str0ng-Passw0rd!");

        Assert.Equal(menuCount, _db.Context.MenuItems.Count());
        Assert.Single(Users.Users);
    }
}
