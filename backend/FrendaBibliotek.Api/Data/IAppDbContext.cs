using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FrendaBibliotek.Api.Data;

/// <summary>
/// Abstraction over AppDbContext. Inject this into services and controllers
/// instead of the concrete type so implementations can be swapped in unit tests
/// without spinning up a real database.
/// </summary>
public interface IAppDbContext
{
    DbSet<Book> Books { get; }
    DbSet<BookCopy> BookCopies { get; }
    DbSet<User> Users { get; }
    DbSet<Loan> Loans { get; }

    /// <summary>Provides access to database-level operations such as transactions.</summary>
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
