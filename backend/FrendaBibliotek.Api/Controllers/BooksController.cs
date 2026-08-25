using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IAppDbContext _db;

    public BooksController(IAppDbContext db) => _db = db;

    // GET /api/books?genreId=5&authorId=3
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? genreId, [FromQuery] int? authorId)
    {
        // Collect genre IDs (selected + all descendants) for filtering
        HashSet<int>? genreIds = null;
        if (genreId.HasValue)
        {
            genreIds = await GetDescendantIds(genreId.Value);
        }

        var query = _db.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.Copies)
                .ThenInclude(c => c.Loans)
            .AsQueryable();

        if (genreIds is not null)
            query = query.Where(b => genreIds.Contains(b.GenreId));

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        var books = await query.ToListAsync();

        var result = books.Select(b =>
        {
            var totalCopies = b.Copies.Count;
            var activeLoanCount = b.Copies.Sum(c => c.Loans.Count(l => l.ReturnedAt == null));
            var availableCopies = totalCopies - activeLoanCount;

            var completedLoans = b.Copies
                .SelectMany(c => c.Loans)
                .Where(l => l.ReturnedAt.HasValue)
                .ToList();

            double? avgDays = completedLoans.Count > 0
                ? completedLoans.Average(l => (l.ReturnedAt!.Value - l.BorrowedAt).TotalDays)
                : null;

            return new BookSummaryDto(b.Id, b.ISBN, b.Title, b.Author.Name, b.AuthorId, b.Genre.Name, b.GenreId,
                b.PublishedYear, b.TotalPages, b.CoverUrl, totalCopies, availableCopies, avgDays);
        });

        return Ok(result);
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

        // Collaborative-filtering recommendations
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
