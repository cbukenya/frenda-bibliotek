using FrendaBibliotek.Api.Data;

namespace FrendaBibliotek.Api.Services;

public class LoanService : ILoanService
{
    private readonly AppDbContext _db;

    public LoanService(AppDbContext db) => _db = db;

    // TODO: implement GetMyLoans, BorrowBook, ReturnLoan
}
