using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Api.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetMyLoansAsync(int userId);
    Task<LoanDto> BorrowBookAsync(int userId, int bookId);
    Task<LoanDto> ReturnLoanAsync(int userId, int loanId);
}
