using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Services;

public class BookService : IBookService
{
    private readonly IAppDbContext _db;

    public BookService(IAppDbContext db) => _db = db;

    // ─── Book Queries ─────────────────────────────────────────────────────────

    public async Task<PagedResult<BookSummaryDto>> GetBooksPagedAsync(
        int? genreId, int? authorId, int page, int pageSize)
    {
        HashSet<int>? genreIds = null;
        if (genreId.HasValue)
            genreIds = await GetDescendantIds(genreId.Value);

        var query = _db.Books.AsQueryable();

        if (genreIds is not null)
            query = query.Where(b => genreIds.Contains(b.GenreId));

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookSummaryDto(
                b.Id, b.ISBN, b.Title, b.Author.Name, b.AuthorId,
                b.Genre.Name, b.GenreId, b.PublishedYear, b.TotalPages, b.CoverUrl,
                b.Copies.Count(),
                b.Copies.Count(c => !c.Loans.Any(l => l.ReturnedAt == null)),
                null
            ))
            .ToListAsync();

        return new PagedResult<BookSummaryDto>(items, total, page, pageSize);
    }

    public async Task<IEnumerable<BookSummaryDto>> GetTopBooksAsync(int count = 10)
    {
        return await _db.Books
            .Select(b => new
            {
                Book = b,
                LoanCount = b.Copies.SelectMany(c => c.Loans).Count()
            })
            .OrderByDescending(x => x.LoanCount)
            .Take(count)
            .Select(x => new BookSummaryDto(
                x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId,
                x.Book.Genre.Name, x.Book.GenreId, x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                x.Book.Copies.Count,
                x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                null
            ))
            .ToListAsync();
    }

    public async Task<BookDetailDto?> GetBookDetailAsync(int id)
    {
        var book = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.Copies)
                .ThenInclude(c => c.Loans)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null) return null;

        var totalCopies = book.Copies.Count;
        var activeLoanCount = book.Copies.Sum(c => c.Loans.Count(l => l.ReturnedAt == null));
        var availableCopies = totalCopies - activeLoanCount;

        var completedLoans = book.Copies
            .SelectMany(c => c.Loans)
            .Where(l => l.ReturnedAt.HasValue)
            .ToList();

        double? avgDays = completedLoans.Count > 0
            ? completedLoans.Average(l => (l.ReturnedAt!.Value - l.BorrowedAt).TotalDays)
            : null;

        var recommendations = await GetRecommendationsAsync(id, book.GenreId);

        return new BookDetailDto(
            book.Id, book.ISBN, book.Title, book.Author.Name, book.AuthorId,
            book.Genre.Name, book.GenreId, book.Description,
            book.PublishedYear, book.TotalPages, book.CoverUrl,
            totalCopies, availableCopies, avgDays, recommendations
        );
    }

    // ─── Loans ────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<LoanDto>> GetMyLoansAsync(int userId)
    {
        var loans = await _db.Loans
            .Where(l => l.UserId == userId)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
                    .ThenInclude(b => b.Author)
            .OrderByDescending(l => l.BorrowedAt)
            .ToListAsync();

        return loans.Select(ToDto);
    }

    public async Task<LoanDto> BorrowBookAsync(int userId, string isbn, DateTime? dueDate = null)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var copy = await _db.BookCopies
            .Where(c => c.Book.ISBN == isbn &&
                        !c.Loans.Any(l => l.ReturnedAt == null))
            .Include(c => c.Book)
                .ThenInclude(b => b.Author)
            .FirstOrDefaultAsync()
            ?? throw new BookNotAvailableException(isbn);

        var now = DateTime.UtcNow;
        var loan = new Loan
        {
            BookCopyId = copy.Id,
            UserId = userId,
            BorrowedAt = now,
            DueDate = dueDate?.ToUniversalTime() ?? now.AddDays(14),
        };

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var created = await _db.Loans
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
                    .ThenInclude(b => b.Author)
            .FirstAsync(l => l.Id == loan.Id);

        return ToDto(created);
    }

    public async Task<LoanDto> ReturnLoanAsync(int userId, int loanId)
    {
        var loan = await _db.Loans
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
                    .ThenInclude(b => b.Author)
            .FirstOrDefaultAsync(l => l.Id == loanId)
            ?? throw new LoanNotFoundException(loanId);

        if (loan.UserId != userId)
            throw new LoanForbiddenException();

        if (loan.ReturnedAt is not null)
            throw new LoanAlreadyReturnedException(loanId);

        loan.ReturnedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ToDto(loan);
    }

    // ─── Private Helpers ──────────────────────────────────────────────────────

    private async Task<List<BookSummaryDto>> GetRecommendationsAsync(int bookId, int genreId)
    {
        var borrowerIds = await _db.Loans
            .Where(l => l.BookCopy.BookId == bookId)
            .Select(l => l.UserId)
            .Distinct()
            .ToListAsync();

        var recommendations = await _db.Books
            .Where(b => b.Id != bookId &&
                        b.Copies.Any(c => c.Loans.Any(l => borrowerIds.Contains(l.UserId))))
            .Select(b => new
            {
                Book = b,
                SharedBorrowers = b.Copies
                    .SelectMany(c => c.Loans)
                    .Where(l => borrowerIds.Contains(l.UserId))
                    .Select(l => l.UserId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.SharedBorrowers)
            .Take(5)
            .Select(x => new BookSummaryDto(
                x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId,
                x.Book.Genre.Name, x.Book.GenreId, x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                x.Book.Copies.Count,
                x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                null
            ))
            .ToListAsync();

        // Genre-based fallback if no collaborative recommendations
        if (!recommendations.Any())
        {
            recommendations = await _db.Books
                .Where(b => b.Id != bookId && b.GenreId == genreId)
                .Select(b => new
                {
                    Book = b,
                    TotalLoans = b.Copies.SelectMany(c => c.Loans).Count()
                })
                .OrderByDescending(x => x.TotalLoans)
                .Take(5)
                .Select(x => new BookSummaryDto(
                    x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId,
                    x.Book.Genre.Name, x.Book.GenreId, x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                    x.Book.Copies.Count,
                    x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                    null
                ))
                .ToListAsync();
        }

        return recommendations;
    }

    /// <summary>Recursively collect a genre ID and all its descendant IDs.</summary>
    private async Task<HashSet<int>> GetDescendantIds(int rootId)
    {
        var allGenres = await _db.Genres.Select(g => new { g.Id, g.ParentId }).ToListAsync();
        var result = new HashSet<int> { rootId };
        var queue = new Queue<int>();
        queue.Enqueue(rootId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var child in allGenres.Where(g => g.ParentId == current))
            {
                if (result.Add(child.Id))
                    queue.Enqueue(child.Id);
            }
        }

        return result;
    }

    private static LoanDto ToDto(Loan l) => new(
        l.Id,
        l.BookCopy.BookId,
        l.BookCopy.Book.ISBN,
        l.BookCopy.Book.Title,
        l.BookCopy.Book.Author.Name,
        l.BookCopy.Book.CoverUrl,
        l.BorrowedAt,
        l.DueDate,
        l.ReturnedAt
    );
}
