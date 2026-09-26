using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record InstrumentDto(
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
    public static InstrumentDto From(Instrument i) => new(
        i.Id, i.Slug, i.Name, i.Section, i.Price, i.OldPrice, i.Installments, i.Description, i.ImageUrl,
        string.IsNullOrWhiteSpace(i.GalleryUrls)
            ? Array.Empty<string>()
            : i.GalleryUrls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}