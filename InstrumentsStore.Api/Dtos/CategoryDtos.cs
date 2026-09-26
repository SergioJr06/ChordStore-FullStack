using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record CategoryDto(int Id, string Name, string? Description, int ProductCount)
{
    public static CategoryDto From(Category c) => new(c.Id, c.Name, c.Description, c.Instruments?.Count ?? 0);
}

public record CategorySaveDto(string Name, string? Description);
