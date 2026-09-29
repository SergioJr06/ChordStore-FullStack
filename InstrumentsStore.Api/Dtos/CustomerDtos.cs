using System;
using InstrumentsStore.Api.Models;
// este codigo é usado para definir os DTOs (Data Transfer Objects) relacionados aos clientes na API da loja de instrumentos.
// Ele inclui a definição de dois registros: `CustomerDto` e `CustomerSaveDto`.
namespace InstrumentsStore.Api.Dtos;

public record CustomerDto( // Vai definir um registro chamado `CustomerDto` que representa os dados de um cliente.
    int Id,
    string Name,
    string? Email,
    string? Phone,
    string? Document,
    string? Address,
    DateTime CreatedAt,
    int SalesCount)
{
    public static CustomerDto From(Customer c) => // define um método estático chamado `From` que recebe um objeto `Customer` e retorna uma instância de `CustomerDto`.
        new(c.Id, c.Name, c.Email, c.Phone, c.Document, c.Address, c.CreatedAt, c.Sales?.Count ?? 0);
}

public record CustomerSaveDto(string Name, string? Email, string? Phone, string? Document, string? Address); // Define outro registro chamado `CustomerSaveDto`, que é usado para salvar ou atualizar informações de um cliente.
                                                                                                             // Ele contém os campos necessários para criar ou atualizar um cliente, como nome, email, telefone, documento e endereço.
