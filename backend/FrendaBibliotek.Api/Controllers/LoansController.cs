using FrendaBibliotek.Api.DTOs;
using FrendaBibliotek.Api.Middleware;
using FrendaBibliotek.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly UserContext _userContext;

    public LoansController(IBookService bookService, UserContext userContext)
    {
        _bookService = bookService;
        _userContext = userContext;
    }

    // GET /api/loans
    [HttpGet]
    public async Task<IActionResult> GetMyLoans()
    {
        var loans = await _bookService.GetMyLoansAsync(_userContext.UserId);
        return Ok(loans);
    }

    // POST /api/loans
    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowRequest request)
    {
        try
        {
            var loan = await _bookService.BorrowBookAsync(_userContext.UserId, request.ISBN);
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
        try
        {
            var loan = await _bookService.ReturnLoanAsync(_userContext.UserId, id);
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
