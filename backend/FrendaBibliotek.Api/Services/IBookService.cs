using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Api.Services;

public interface IBookService
{
    Task<PagedResult<BookSummaryDto>> GetBooksPagedAsync(int? genreId, int? authorId, int page, int pageSize);
    Task<IEnumerable<BookSummaryDto>> GetTopBooksAsync(int count = 10);
    Task<BookDetailDto?> GetBookDetailAsync(int id);
    Task<IEnumerable<LoanDto>> GetMyLoansAsync(int userId);
    Task<LoanDto> BorrowBookAsync(int userId, string isbn, DateTime? dueDate = null);
    Task<LoanDto> ReturnLoanAsync(int userId, int loanId);
}
