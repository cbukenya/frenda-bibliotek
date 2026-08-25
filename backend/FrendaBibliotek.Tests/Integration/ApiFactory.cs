using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.Data.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.PostgreSql;

namespace FrendaBibliotek.Tests.Integration;

/// <summary>
/// Shared test fixture that spins up a real PostgreSQL container via Testcontainers
/// and hosts the full API using WebApplicationFactory.
/// One container per test collection — fast and isolated.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestJwtSecret = "frenda-bibliotek-dev-secret-key-min-32-chars!!";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("bibliotek_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("JWT_SECRET", TestJwtSecret);

        builder.ConfigureServices(services =>
        {
            // Replace the registered DbContext with one pointing to the test container
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(_postgres.GetConnectionString()));

            // Run migrations and seed on the test DB
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            DataSeeder.SeedAsync(db).GetAwaiter().GetResult();
        });
    }

    /// <summary>
    /// Creates a JWT token for the given user ID, usable for authenticated test requests.
    /// </summary>
    public static string CreateTestToken(int userId, string email = "test@test.se", string name = "Test User")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name),
        };

        var token = new JwtSecurityToken(
            issuer: "frenda-bibliotek",
            audience: "frenda-bibliotek",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
