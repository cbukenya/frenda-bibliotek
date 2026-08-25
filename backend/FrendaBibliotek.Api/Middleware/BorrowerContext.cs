namespace FrendaBibliotek.Api.Middleware;

/// <summary>
/// Scoped service that holds the current user's ID for the duration of a request.
/// Populated by <see cref="UserContextMiddleware"/> from the X-User-Id header.
/// </summary>
public class UserContext
{
    public int UserId { get; private set; }
    public bool IsSet { get; private set; }

    public void Set(int userId)
    {
        UserId = userId;
        IsSet = true;
    }
}

/// <summary>
/// Middleware that reads the X-User-Id header and populates <see cref="UserContext"/>.
/// Returns 400 if the header is missing or not a valid integer on routes that require it.
/// </summary>
public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        if (context.Request.Headers.TryGetValue("X-User-Id", out var value)
            && int.TryParse(value, out var userId))
        {
            userContext.Set(userId);
        }

        await _next(context);
    }
}
