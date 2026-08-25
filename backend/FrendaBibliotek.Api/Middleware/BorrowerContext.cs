namespace FrendaBibliotek.Api.Middleware;

// Extracts the current borrower from the X-Borrower-Id header
// and makes it available as a scoped service throughout the request pipeline.

public class BorrowerContext
{
    public int BorrowerId { get; private set; }

    // TODO: implement SetFromHeader
}

public class BorrowerContextMiddleware
{
    private readonly RequestDelegate _next;

    public BorrowerContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, BorrowerContext borrowerContext)
    {
        // TODO: extract X-Borrower-Id and populate BorrowerContext
        await _next(context);
    }
}
