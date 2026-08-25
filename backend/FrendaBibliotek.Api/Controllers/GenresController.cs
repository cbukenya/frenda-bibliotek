using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GenresController : ControllerBase
{
    private readonly IAppDbContext _db;

    public GenresController(IAppDbContext db) => _db = db;

    // GET /api/genres — flat list
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var genres = await _db.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto(g.Id, g.Name, g.Slug, g.ParentId, null))
            .ToListAsync();

        return Ok(genres);
    }

    // GET /api/genres/tree — nested tree (roots with children)
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var all = await _db.Genres.OrderBy(g => g.Name).ToListAsync();
        var lookup = all.ToLookup(g => g.ParentId);

        IEnumerable<GenreDto> BuildTree(int? parentId) =>
            lookup[parentId].Select(g => new GenreDto(
                g.Id, g.Name, g.Slug, g.ParentId, BuildTree(g.Id).ToList()
            ));

        return Ok(BuildTree(null));
    }

    // GET /api/genres/{id}/children — direct children of a genre
    [HttpGet("{id}/children")]
    public async Task<IActionResult> GetChildren(int id)
    {
        var children = await _db.Genres
            .Where(g => g.ParentId == id)
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto(g.Id, g.Name, g.Slug, g.ParentId, null))
            .ToListAsync();

        return Ok(children);
    }

    // GET /api/genres/{id}/ancestors — breadcrumb path from root to this genre
    [HttpGet("{id}/ancestors")]
    public async Task<IActionResult> GetAncestors(int id)
    {
        var all = await _db.Genres.ToListAsync();
        var dict = all.ToDictionary(g => g.Id);
        var path = new List<GenreDto>();

        var current = dict.GetValueOrDefault(id);
        while (current is not null)
        {
            path.Insert(0, new GenreDto(current.Id, current.Name, current.Slug, current.ParentId));
            current = current.ParentId.HasValue ? dict.GetValueOrDefault(current.ParentId.Value) : null;
        }

        return Ok(path);
    }
}
