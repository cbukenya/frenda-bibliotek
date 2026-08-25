using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IAppDbContext _db;

    public AuthorsController(IAppDbContext db) => _db = db;

    // GET /api/authors
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var authors = await _db.Authors
            .OrderBy(a => a.Name)
            .Select(a => new AuthorDto(a.Id, a.Name, a.Slug))
            .ToListAsync();

        return Ok(authors);
    }

    // GET /api/authors/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var author = await _db.Authors.FindAsync(id);
        if (author is null) return NotFound();
        return Ok(new AuthorDto(author.Id, author.Name, author.Slug));
    }
}
