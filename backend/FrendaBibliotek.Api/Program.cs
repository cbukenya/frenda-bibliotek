using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.Data.Seed;
using FrendaBibliotek.Api.Middleware;
using FrendaBibliotek.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ─────────────────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Frenda Bibliotek API",
        Version = "v1",
        Description = "Book lending API — browse books, manage loans, and discover recommendations.",
    });

    c.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
    {
        Name = "X-User-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Simulated user identity. Enter the ID of the borrower (e.g. 1 = Alice Lindgren).",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "UserId" }
            },
            Array.Empty<string>()
        }
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("No connection string configured.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// Register the interface so controllers and services can depend on IAppDbContext
// rather than the concrete type, enabling unit testing without a real database.
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Application services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<UserContext>();

// CORS — allow frontend dev server
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:3000")
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

// ─── Migrations + Seed ────────────────────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}

// ─── Middleware ───────────────────────────────────────────────────────────────

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Frenda Bibliotek API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseMiddleware<UserContextMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Expose Program to WebApplicationFactory in the test project
public partial class Program { }

