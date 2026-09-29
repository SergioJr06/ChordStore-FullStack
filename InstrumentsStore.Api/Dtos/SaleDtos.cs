using System;
using System.Collections.Generic;
using System.Linq;
using InstrumentsStore.Api.Models;
// Define os DTOs (Data Transfer Objects) para vendas na API da loja de instrumentos.
// DTOs são usados para transferir dados entre o cliente e o servidor, encapsulando as informações relevantes de uma venda e seus itens.
namespace InstrumentsStore.Api.Dtos;

public record SaleItemDto(int InstrumentId, string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal)
{
    public static SaleItemDto From(SaleItem i) => // Cria um SaleItemDto a partir de um SaleItem do modelo de domínio.
        new(i.InstrumentId, i.ProductNameSnapshot, i.UnitPrice, i.Quantity, i.Subtotal);
}

public record SaleDto( // Representa uma venda completa, incluindo informações do cliente, status, método de pagamento e itens da venda.
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

public record SaleItemInputDto(int InstrumentId, int Quantity); // Representa os dados de entrada para um item de venda, incluindo o ID do instrumento e a quantidade desejada.

public record SaleCreateDto(
    int? CustomerId,
    string? CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    string PaymentMethod,
    int Installments,
    List<SaleItemInputDto> Items); // Representa os dados de entrada para criar uma nova venda, incluindo informações do cliente, método de pagamento, parcelas e itens da venda.

public record SaleStatusUpdateDto(string Status);

public record CheckoutDto( // Representa os dados de entrada para o processo de checkout, incluindo informações do cliente, método de pagamento, parcelas e itens da venda.
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string? CustomerAddress,
    string PaymentMethod,
    int Installments,
    List<SaleItemInputDto> Items);

public record CheckoutResultDto(int OrderId, decimal Total, DateTime CreatedAt); // Representa o resultado do processo de checkout, incluindo o ID do pedido, o valor total e a data de criação da venda.
