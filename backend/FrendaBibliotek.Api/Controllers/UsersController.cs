using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(u.Id, u.Name, u.Email))
            .ToListAsync();

        return Ok(users);
    }
}
