using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrendaBibliotek.Api.Data;
using FrendaBibliotek.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FrendaBibliotek.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(IAppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Name, email, and password are required." });
        }

        if (request.Password.Length < 6)
            return BadRequest(new { error = "Password must be at least 6 characters." });

        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            return Conflict(new { error = "An account with this email already exists." });

        var user = new Models.User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateToken(user);
        return Ok(new AuthResponse(token, new UserDto(user.Id, user.Name, user.Email)));
    }

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and password are required." });

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password." });

        var token = GenerateToken(user);
        return Ok(new AuthResponse(token, new UserDto(user.Id, user.Name, user.Email)));
    }

    // GET /api/auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound();
        return Ok(new UserDto(user.Id, user.Name, user.Email));
    }

    private string GenerateToken(Models.User user)
    {
        var secret = _config["JWT_SECRET"] ?? "frenda-bibliotek-dev-secret-key-min-32-chars!!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        var token = new JwtSecurityToken(
            issuer: "frenda-bibliotek",
            audience: "frenda-bibliotek",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
