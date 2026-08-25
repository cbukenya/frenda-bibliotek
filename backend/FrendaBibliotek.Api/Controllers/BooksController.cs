using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;

    public BooksController(AppDbContext db) => _db = db;

    // GET /api/books
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _db.Books
            .Include(b => b.Copies)
                .ThenInclude(c => c.Loans)
            .ToListAsync();

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

            return new BookSummaryDto(b.Id, b.Title, b.Author, b.Genre, b.PublishedYear,
                b.CoverUrl, totalCopies, availableCopies, avgDays);
        });

        return Ok(result);
    }

    // GET /api/books/top
    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var top = await _db.Books
            .Select(b => new
            {
                Book = b,
                LoanCount = b.Copies.SelectMany(c => c.Loans).Count()
            })
            .OrderByDescending(x => x.LoanCount)
            .Take(10)
            .Select(x => new BookSummaryDto(
                x.Book.Id, x.Book.Title, x.Book.Author, x.Book.Genre,
                x.Book.PublishedYear, x.Book.CoverUrl,
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

        // Collaborative-filtering recommendations:
        // Find users who borrowed this book, then find other books they also borrowed
        var borrowerIds = await _db.Loans
            .Where(l => l.BookCopy.BookId == id)
            .Select(l => l.UserId)
            .Distinct()
            .ToListAsync();

        var recommendations = await _db.Books
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
                x.Book.Id, x.Book.Title, x.Book.Author, x.Book.Genre,
                x.Book.PublishedYear, x.Book.CoverUrl,
                x.Book.Copies.Count,
                x.Book.Copies.Count - x.Book.Copies.SelectMany(c => c.Loans).Count(l => l.ReturnedAt == null),
                null
            ))
            .ToListAsync();

        var detail = new BookDetailDto(
            book.Id, book.Title, book.Author, book.Genre, book.Description,
            book.PublishedYear, book.TotalPages, book.CoverUrl,
            totalCopies, availableCopies, avgDays, recommendations
        );

        return Ok(detail);
    }
}
