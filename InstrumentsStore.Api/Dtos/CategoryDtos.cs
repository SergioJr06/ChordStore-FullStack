using InstrumentsStore.Api.Models;
// Este código define os DTOs (Data Transfer Objects) para a entidade Category, usados na API do InstrumentsStore.  
namespace InstrumentsStore.Api.Dtos;
// DTO de categoria para exibição na API, contendo ID, nome, descrição e contagem de produtos associados.
public record CategoryDto(int Id, string Name, string? Description, int ProductCount)
{
    public static CategoryDto From(Category c) => new(c.Id, c.Name, c.Description, c.Instruments?.Count ?? 0); // Construtor estático que cria um CategoryDto a partir de uma entidade Category, calculando a contagem de produtos associados.
}
// DTO de categoria para salvar na API (criação ou atualização), contendo nome e descrição.
public record CategorySaveDto(string Name, string? Description);
