using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Services;

public class BookService : IBookService
{
    private readonly IAppDbContext _db;

    public BookService(IAppDbContext db) => _db = db;

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

        // Find first available copy for the given ISBN (no active loan)
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

        // Re-query with navigation properties for the response DTO
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
