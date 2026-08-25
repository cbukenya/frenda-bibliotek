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
        return await _db.Loans
            .Where(l => l.UserId == userId)
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .OrderByDescending(l => l.BorrowedAt)
            .Select(l => ToDto(l))
            .ToListAsync();
    }

    public async Task<LoanDto> BorrowBookAsync(int userId, string isbn)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        // Find first available copy for the given ISBN (no active loan)
        var copy = await _db.BookCopies
            .Where(c => c.Book.ISBN == isbn &&
                        !c.Loans.Any(l => l.ReturnedAt == null))
            .Include(c => c.Book)
            .FirstOrDefaultAsync()
            ?? throw new BookNotAvailableException(isbn);

        var loan = new Loan
        {
            BookCopyId = copy.Id,
            UserId = userId,
            BorrowedAt = DateTime.UtcNow,
        };

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        // Re-query with navigation properties for the response DTO
        var created = await _db.Loans
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
            .FirstAsync(l => l.Id == loan.Id);

        return ToDto(created);
    }

    public async Task<LoanDto> ReturnLoanAsync(int userId, int loanId)
    {
        var loan = await _db.Loans
            .Include(l => l.BookCopy)
                .ThenInclude(c => c.Book)
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
        l.BookCopy.Book.Author,
        l.BookCopy.Book.CoverUrl,
        l.BorrowedAt,
        l.ReturnedAt
    );
}
