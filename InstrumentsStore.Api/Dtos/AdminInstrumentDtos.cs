using System;
using System.Linq;
using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

/// <summary>Representação completa do produto, usada só nas telas do admin.</summary>
public record AdminInstrumentDto(
    int Id,
    string Slug,
    string Name,
    string Section,
    decimal? Price,
    decimal? OldPrice,
    int Installments,
    string? Description,
    string ImageUrl,
    string[] GalleryUrls,
    string Brand,
    int StockQuantity,
    int? CategoryId,
    string? CategoryName,
    int? SupplierId,
    string? SupplierName)
{
    public static AdminInstrumentDto From(Instrument i) => new(
        i.Id, i.Slug, i.Name, i.Section, i.Price, i.OldPrice, i.Installments, i.Description, i.ImageUrl,
        string.IsNullOrWhiteSpace(i.GalleryUrls)
            ? Array.Empty<string>()
            : i.GalleryUrls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        i.Brand, i.StockQuantity,
        i.CategoryId, i.Category?.Name,
        i.SupplierId, i.Supplier?.Name);
}

/// <summary>Payload de criação/edição enviado pelo formulário de produto do admin.</summary>
public record AdminInstrumentSaveDto(
    string Slug,
    string Name,
    string Section,
    decimal? Price,
    decimal? OldPrice,
    int Installments,
    string? Description,
    string ImageUrl,
    string[]? GalleryUrls,
    string Brand,
    int StockQuantity,
    int? CategoryId,
    int? SupplierId)
{
    public string GalleryUrlsAsString() =>
        GalleryUrls == null || GalleryUrls.Length == 0 ? "" : string.Join(';', GalleryUrls.Where(g => !string.IsNullOrWhiteSpace(g)));
}
