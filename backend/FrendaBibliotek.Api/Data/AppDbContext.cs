using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
