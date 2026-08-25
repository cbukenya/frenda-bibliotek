using System.Security.Claims;

namespace FrendaBibliotek.Api.Middleware;

/// <summary>
/// Scoped service that holds the current user's ID for the duration of a request.
/// Populated by <see cref="UserContextMiddleware"/> from JWT claims.
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
/// Middleware that reads the user ID from JWT claims and populates <see cref="UserContext"/>.
/// </summary>
public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is not null && int.TryParse(claim.Value, out var userId))
        {
            userContext.Set(userId);
        }

        await _next(context);
    }
}
