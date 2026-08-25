using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Api.Services;

public interface IBookService
{
    Task<IEnumerable<LoanDto>> GetMyLoansAsync(int userId);
    Task<LoanDto> BorrowBookAsync(int userId, string isbn);
    Task<LoanDto> ReturnLoanAsync(int userId, int loanId);
}
