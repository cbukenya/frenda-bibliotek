using FrendaBibliotek.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FrendaBibliotek.Api;

/// <summary>
/// Used by dotnet-ef at design time (migrations add, migrations script) to
/// instantiate AppDbContext without a live database connection.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=bibliotek;Username=bibliotek_user;Password=change_me")
            .Options;

        return new AppDbContext(options);
    }
}
