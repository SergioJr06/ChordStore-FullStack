using InstrumentsStore.Api.Models;
// Este código define um record chamado `InstrumentDto` que representa um objeto de transferência de dados (DTO) para instrumentos musicais na API da loja de instrumentos.
// Ele contém propriedades como `Id`, `Slug`, `Name`, `Section`, `Price`, `OldPrice`, `Installments`, `Description`, `ImageUrl` e `GalleryUrls`.
namespace InstrumentsStore.Api.Dtos;

public record InstrumentDto(  // Define a record type named `InstrumentDto` that represents the data transfer object for a musical instrument.
    int Id,
    string Slug,
    string Name,
    string Section,
    decimal? Price,
    decimal? OldPrice,
    int Installments,
    string? Description,
    string ImageUrl,
    string[] GalleryUrls)
{
    public static InstrumentDto From(Instrument i) => new( // Cria uma nova instância de `InstrumentDto` a partir de um objeto `Instrument`.
        i.Id, i.Slug, i.Name, i.Section, i.Price, i.OldPrice, i.Installments, i.Description, i.ImageUrl,
        string.IsNullOrWhiteSpace(i.GalleryUrls)
            ? Array.Empty<string>()
            : i.GalleryUrls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)); // Divide a string de URLs da galeria em um array de strings, removendo entradas vazias e espaços em branco.
}