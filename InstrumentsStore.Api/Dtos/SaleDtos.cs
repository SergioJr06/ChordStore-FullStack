using System;
using System.Collections.Generic;
using System.Linq;
using InstrumentsStore.Api.Models;

namespace InstrumentsStore.Api.Dtos;

public record SaleItemDto(int InstrumentId, string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal)
{
    public static SaleItemDto From(SaleItem i) =>
        new(i.InstrumentId, i.ProductNameSnapshot, i.UnitPrice, i.Quantity, i.Subtotal);
}

public record SaleDto(
    int Id,
    int? CustomerId,
    string CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    DateTime CreatedAt,
    string Status,
    string PaymentMethod,
    int Installments,
    decimal Total,
    string Origin,
    List<SaleItemDto> Items)
{
    public static SaleDto From(Sale s) => new(
        s.Id, s.CustomerId, s.CustomerNameSnapshot, s.CustomerEmailSnapshot, s.CustomerPhoneSnapshot,
        s.CreatedAt, s.Status.ToString(), s.PaymentMethod.ToString(), s.Installments, s.Total, s.Origin,
        s.Items.Select(SaleItemDto.From).ToList());
}

public record SaleItemInputDto(int InstrumentId, int Quantity);

/// <summary>Usado pelo admin para lançar uma venda manualmente (tipo PDV).</summary>
public record SaleCreateDto(
    int? CustomerId,
    string? CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    string PaymentMethod,
    int Installments,
    List<SaleItemInputDto> Items);

public record SaleStatusUpdateDto(string Status);

/// <summary>Usado pelo checkout público (carrinho do site do cliente).</summary>
public record CheckoutDto(
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string? CustomerAddress,
    string PaymentMethod,
    int Installments,
    List<SaleItemInputDto> Items);

public record CheckoutResultDto(int OrderId, decimal Total, DateTime CreatedAt);
