using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record SupplierDto(
    int Id,
    string Name,
    string? Document,
    string? Phone,
    string? Email,
    string? Address,
    int ProductCount)
{
    public static SupplierDto From(Supplier s) =>
        new(s.Id, s.Name, s.Document, s.Phone, s.Email, s.Address, s.Instruments?.Count ?? 0);
}

public record SupplierSaveDto(string Name, string? Document, string? Phone, string? Email, string? Address);
