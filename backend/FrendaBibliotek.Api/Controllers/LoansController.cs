using FrendaBibliotek.Api.DTOs;
using FrendaBibliotek.Api.Middleware;
using FrendaBibliotek.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly UserContext _userContext;

    public LoansController(ILoanService loanService, UserContext userContext)
    {
        _loanService = loanService;
        _userContext = userContext;
    }

    private IActionResult RequireUser(out int userId)
    {
        userId = _userContext.UserId;
        if (!_userContext.IsSet)
            return BadRequest(new { error = "X-User-Id header is required." });
        return null!;
    }

    // GET /api/loans
    [HttpGet]
    public async Task<IActionResult> GetMyLoans()
    {
        var guard = RequireUser(out var userId);
        if (guard is not null) return guard;

        var loans = await _loanService.GetMyLoansAsync(userId);
        return Ok(loans);
    }

    // POST /api/loans
    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowRequest request)
    {
        var guard = RequireUser(out var userId);
        if (guard is not null) return guard;

        try
        {
            var loan = await _loanService.BorrowBookAsync(userId, request.BookId);
            return CreatedAtAction(nameof(GetMyLoans), loan);
        }
        catch (BookNotAvailableException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // PATCH /api/loans/{id}/return
    [HttpPatch("{id}/return")]
    public async Task<IActionResult> Return(int id)
    {
        var guard = RequireUser(out var userId);
        if (guard is not null) return guard;

        try
        {
            var loan = await _loanService.ReturnLoanAsync(userId, id);
            return Ok(loan);
        }
        catch (LoanNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (LoanForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (LoanAlreadyReturnedException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
