using System;
using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record CustomerDto(
    int Id,
    string Name,
    string? Email,
    string? Phone,
    string? Document,
    string? Address,
    DateTime CreatedAt,
    int SalesCount)
{
    public static CustomerDto From(Customer c) =>
        new(c.Id, c.Name, c.Email, c.Phone, c.Document, c.Address, c.CreatedAt, c.Sales?.Count ?? 0);
}

public record CustomerSaveDto(string Name, string? Email, string? Phone, string? Document, string? Address);
