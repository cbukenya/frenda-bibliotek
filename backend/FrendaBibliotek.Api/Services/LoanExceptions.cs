namespace FrendaBibliotek.Api.Services;

// Domain exceptions — map to HTTP status codes in the controllers

public class BookNotAvailableException : Exception
{
    public BookNotAvailableException(int bookId)
        : base($"No copies of book {bookId} are currently available.") { }
}

public class LoanNotFoundException : Exception
{
    public LoanNotFoundException(int loanId)
        : base($"Loan {loanId} not found.") { }
}

public class LoanForbiddenException : Exception
{
    public LoanForbiddenException()
        : base("You are not authorised to modify this loan.") { }
}

public class LoanAlreadyReturnedException : Exception
{
    public LoanAlreadyReturnedException(int loanId)
        : base($"Loan {loanId} has already been returned.") { }
}
