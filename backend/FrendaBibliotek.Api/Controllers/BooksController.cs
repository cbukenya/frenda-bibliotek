using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IAppDbContext _db;

    public BooksController(IAppDbContext db) => _db = db;

    // GET /api/books?genreId=5&authorId=3&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? genreId,
        [FromQuery] int? authorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Collect genre IDs (selected + all descendants) for filtering
        HashSet<int>? genreIds = null;
        if (genreId.HasValue)
            genreIds = await GetDescendantIds(genreId.Value);

        // Build a filterable base query — no Include needed; Select does the JOINs
        var query = _db.Books.AsQueryable();

        if (genreIds is not null)
            query = query.Where(b => genreIds.Contains(b.GenreId));

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        // Total count for pagination metadata (runs as COUNT(*) in SQL)
        var total = await query.CountAsync();

        // Server-side projection: counts happen in PostgreSQL, not in C#
        var items = await query
            .OrderBy(b => b.Title) // stable sort required for consistent paging
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookSummaryDto(
                b.Id,
                b.ISBN,
                b.Title,
                b.Author.Name,
                b.AuthorId,
                b.Genre.Name,
                b.GenreId,
                b.PublishedYear,
                b.TotalPages,
                b.CoverUrl,
                b.Copies.Count(),
                // EXISTS subquery — no loan rows travel over the wire
                b.Copies.Count(c => !c.Loans.Any(l => l.ReturnedAt == null)),
                null // avgDays omitted in list view for performance; available in detail
            ))
            .ToListAsync();

        return Ok(new PagedResult<BookSummaryDto>(items, total, page, pageSize));
    }

    // GET /api/books/top
    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var top = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Select(b => new
            {
                Book = b,
                LoanCount = b.Copies.SelectMany(c => c.Loans).Count()
            })
            .OrderByDescending(x => x.LoanCount)
            .Take(10)
            .Select(x => new BookSummaryDto(
                x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId, x.Book.Genre.Name, x.Book.GenreId,
                x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                x.Book.Copies.Count,
                x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                null
            ))
            .ToListAsync();

        return Ok(top);
    }

    // GET /api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.Copies)
                .ThenInclude(c => c.Loans)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null) return NotFound();

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

        // Step 1: Collaborative-filtering recommendations (users who also borrowed this book)
        var borrowerIds = await _db.Loans
            .Where(l => l.BookCopy.BookId == id)
            .Select(l => l.UserId)
            .Distinct()
            .ToListAsync();

        var recommendations = await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Where(b => b.Id != id &&
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
                x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId, x.Book.Genre.Name, x.Book.GenreId,
                x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                x.Book.Copies.Count,
                x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                null
            ))
            .ToListAsync();

        // Step 2: Genre-based fallback — if no shared-borrower recommendations exist,
        // suggest popular books from the same genre so the list is never empty.
        if (!recommendations.Any())
        {
            recommendations = await _db.Books
                .Where(b => b.Id != id && b.GenreId == book.GenreId)
                .Select(b => new
                {
                    Book = b,
                    TotalLoans = b.Copies.SelectMany(c => c.Loans).Count()
                })
                .OrderByDescending(x => x.TotalLoans)
                .Take(5)
                .Select(x => new BookSummaryDto(
                    x.Book.Id, x.Book.ISBN, x.Book.Title, x.Book.Author.Name, x.Book.AuthorId, x.Book.Genre.Name, x.Book.GenreId,
                    x.Book.PublishedYear, x.Book.TotalPages, x.Book.CoverUrl,
                    x.Book.Copies.Count,
                    x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                    null
                ))
                .ToListAsync();
        }

        var detail = new BookDetailDto(
            book.Id, book.ISBN, book.Title, book.Author.Name, book.AuthorId, book.Genre.Name, book.GenreId, book.Description,
            book.PublishedYear, book.TotalPages, book.CoverUrl,
            totalCopies, availableCopies, avgDays, recommendations
        );

        return Ok(detail);
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
}
