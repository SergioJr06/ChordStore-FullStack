using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InstrumentsStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.AdminUsers
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Credenciais inválidas" });

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateToken(user.Email, user.Id);

        return Ok(new LoginResponseDto(token, AdminUserDto.From(user)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AdminUserDto>> Me()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();
        return Ok(AdminUserDto.From(user));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<AdminUserDto>> UpdateAccount([FromBody] UpdateAccountDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        var usernameTaken = await _context.AdminUsers.AnyAsync(u => u.Id != user.Id && u.Username == dto.Username);
        if (usernameTaken) return Conflict(new { message = "Esse nome de usuário já está em uso." });

        var emailTaken = await _context.AdminUsers.AnyAsync(u => u.Id != user.Id && u.Email == dto.Email);
        if (emailTaken) return Conflict(new { message = "Esse e-mail já está em uso." });

        user.Username = dto.Username;
        user.Email = dto.Email;
        await _context.SaveChangesAsync();

        return Ok(AdminUserDto.From(user));
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Senha atual incorreta." });

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest(new { message = "A nova senha precisa ter pelo menos 6 caracteres." });

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Models.AdminUser?> GetCurrentUserAsync()
    {
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(email)) return null;
        return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
    }

    private string GenerateToken(string email, int userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
