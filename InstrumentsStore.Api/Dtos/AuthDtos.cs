using InstrumentsStore.Api.Models;
// Este código define os DTOs (Data Transfer Objects) para autenticação e gerenciamento de conta de usuário administrador na API do InstrumentsStore.
namespace InstrumentsStore.Api.Dtos;

public record LoginRequestDto(string Email, string Password); // DTO para requisição de login, contendo email e senha do usuário.
public record LoginResponseDto(string Token, AdminUserDto User); // DTO para resposta de login, contendo o token JWT e informações do usuário administrador.

public record AdminUserDto(int Id, string Username, string Email) // DTO para informações do usuário administrador, contendo ID, nome de usuário e email.
{
    public static AdminUserDto From(AdminUser u) => new(u.Id, u.Username, u.Email); // Método de fábrica para criar um AdminUserDto a partir de um AdminUser.
}

public record UpdateAccountDto(string Username, string Email); // DTO para atualização de conta do usuário administrador, contendo nome de usuário e email.
public record ChangePasswordDto(string CurrentPassword, string NewPassword); // DTO para alteração de senha do usuário administrador, contendo a senha atual e a nova senha.
