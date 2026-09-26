using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record LoginRequestDto(string Email, string Password);
public record LoginResponseDto(string Token, AdminUserDto User);

public record AdminUserDto(int Id, string Username, string Email)
{
    public static AdminUserDto From(AdminUser u) => new(u.Id, u.Username, u.Email);
}

public record UpdateAccountDto(string Username, string Email);
public record ChangePasswordDto(string CurrentPassword, string NewPassword);
