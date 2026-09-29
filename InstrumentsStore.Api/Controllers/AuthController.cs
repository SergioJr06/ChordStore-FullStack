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

// Este código define um controlador de autenticação para uma API, permitindo que os usuários façam login, obtenham informações
// sobre si mesmos e atualizem suas contas. Ele utiliza JWT (JSON Web Tokens) para autenticação e autorização.

namespace InstrumentsStore.Api.Controllers; // Define o namespace do controlador

[ApiController] // Indica que este é um controlador de API
[Route("api/auth")]
public class AuthController : ControllerBase // Controlador base para APIs, não retorna Views
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration config, AppDbContext context) // Injeta as dependências de configuração e contexto do banco de dados
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request) // Define que este método responde a requisições POST para "api/auth/login" e espera um objeto JSON no corpo da requisição
    {
        var user = await _context.AdminUsers
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Credenciais inválidas" });

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(); // Salva a data e hora do último login do usuário no banco de dados

        var token = GenerateToken(user.Email, user.Id);

        return Ok(new LoginResponseDto(token, AdminUserDto.From(user)));
    } // Define que este método responde a requisições POST para "api/auth/login" e espera um objeto JSON no corpo da requisição

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AdminUserDto>> Me() // Define que este método responde a requisições GET para "api/auth/me" e requer autenticação
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();
        return Ok(AdminUserDto.From(user));
    }

    [Authorize] // Define que este método requer autenticação
    [HttpPut("me")]
    public async Task<ActionResult<AdminUserDto>> UpdateAccount([FromBody] UpdateAccountDto dto) // Define que este método responde a requisições PUT para "api/auth/me" e espera um objeto JSON no corpo da requisição
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        var usernameTaken = await _context.AdminUsers.AnyAsync(u => u.Id != user.Id && u.Username == dto.Username); // Verifica se o nome de usuário já está em uso por outro usuário
        if (usernameTaken) return Conflict(new { message = "Esse nome de usuário já está em uso." });

        var emailTaken = await _context.AdminUsers.AnyAsync(u => u.Id != user.Id && u.Email == dto.Email); // Verifica se o e-mail já está em uso por outro usuário
        if (emailTaken) return Conflict(new { message = "Esse e-mail já está em uso." });

        user.Username = dto.Username;
        user.Email = dto.Email;
        await _context.SaveChangesAsync();

        return Ok(AdminUserDto.From(user)); // Retorna os dados atualizados do usuário autenticado
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto) // Define que este método responde a requisições PUT para "api/auth/change-password" e espera um objeto JSON no corpo da requisição
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Senha atual incorreta." });

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest(new { message = "A nova senha precisa ter pelo menos 6 caracteres." }); // Valida se a nova senha atende aos critérios mínimos de segurança

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Models.AdminUser?> GetCurrentUserAsync() // Obtém o usuário autenticado com base no token JWT presente na requisição
    {
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(email)) return null;
        return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email); // Retorna o usuário correspondente ao e-mail encontrado no token JWT
    }

    private string GenerateToken(string email, int userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // Cria as credenciais de assinatura usando a chave secreta e o algoritmo HMAC SHA256

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", userId.ToString())
        };

        var token = new JwtSecurityToken( // Cria o token JWT com as informações fornecidas, incluindo emissor, audiência, claims, tempo de expiração e credenciais de assinatura
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token); // Retorna o token JWT como uma string
    }
}
