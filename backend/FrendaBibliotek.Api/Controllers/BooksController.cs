using FrendaBibliotek.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService) => _bookService = bookService;

    // GET /api/books?genreId=5&authorId=3&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? genreId,
        [FromQuery] int? authorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _bookService.GetBooksPagedAsync(genreId, authorId, page, pageSize);
        return Ok(result);
    }

    // GET /api/books/top
    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var top = await _bookService.GetTopBooksAsync();
        return Ok(top);
    }

    // GET /api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _bookService.GetBookDetailAsync(id);
        if (detail is null) return NotFound();
        return Ok(detail);
    }
}
