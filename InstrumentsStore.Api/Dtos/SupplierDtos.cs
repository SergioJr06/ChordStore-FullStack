using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos; // este codigo é usado para definir os DTOs (Data Transfer Objects) relacionados aos fornecedores na API da loja de instrumentos.

public record SupplierDto( // define os campos que serão retornados para o cliente quando ele solicitar informações sobre um fornecedor específico.
    int Id,
    string Name,
    string? Document,
    string? Phone,
    string? Email,
    string? Address,
    int ProductCount)
{
    public static SupplierDto From(Supplier s) => // método estático que cria uma instância de SupplierDto a partir de um objeto Supplier. Ele mapeia os campos do modelo para o DTO, incluindo a contagem de produtos associados ao fornecedor.
        new(s.Id, s.Name, s.Document, s.Phone, s.Email, s.Address, s.Instruments?.Count ?? 0);
}

public record SupplierSaveDto(string Name, string? Document, string? Phone, string? Email, string? Address); // define os campos que serão recebidos do cliente quando ele enviar informações para criar ou atualizar um fornecedor. Ele inclui o nome, documento, telefone, e-mail e endereço do fornecedor.
