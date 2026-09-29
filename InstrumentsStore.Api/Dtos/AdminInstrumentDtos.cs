using System;
using System.Linq;
using InstrumentsStore.Api.Models;

// este codigo define os DTOs (Data Transfer Objects) para a entidade Instrument, usados na API do admin do InstrumentsStore.

namespace InstrumentsStore.Api.Dtos;

public record AdminInstrumentDto( // DTO de instrumento para exibição no admin
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
    public static AdminInstrumentDto From(Instrument i) => new( // método de fábrica para criar um AdminInstrumentDto a partir de um Instrument
        i.Id, i.Slug, i.Name, i.Section, i.Price, i.OldPrice, i.Installments, i.Description, i.ImageUrl,
        string.IsNullOrWhiteSpace(i.GalleryUrls)
            ? Array.Empty<string>()
            : i.GalleryUrls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        i.Brand, i.StockQuantity,
        i.CategoryId, i.Category?.Name,
        i.SupplierId, i.Supplier?.Name); // cria um array de URLs da galeria a partir da string separada por ponto e vírgula, se não for nula ou vazia
}

// DTO de instrumento para salvar no admin (criação ou atualização)
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
    public string GalleryUrlsAsString() => // método para converter o array de URLs da galeria em uma string separada por ponto e vírgula
        GalleryUrls == null || GalleryUrls.Length == 0 ? "" : string.Join(';', GalleryUrls.Where(g => !string.IsNullOrWhiteSpace(g)));
}
