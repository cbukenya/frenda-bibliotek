using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAppDbContext _db;

    public UsersController(IAppDbContext db) => _db = db;

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
