using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Services;

public static class SaleBuilder
{
    public record Input(
        int? CustomerId,
        string CustomerName,
        string? CustomerEmail,
        string? CustomerPhone,
        string? CustomerAddress,
        string PaymentMethod,
        int Installments,
        List<SaleItemInputDto> Items,
        string Origin);

    /// <summary>
    /// Valida estoque, monta a venda com os itens "congelados" e baixa o estoque.
    /// Não chama SaveChanges — quem chamar decide quando salvar (permite tudo em uma transação).
    /// </summary>
    public static async Task<(Sale? Sale, string? Error)> BuildAsync(AppDbContext context, Input input)
    {
        if (input.Items == null || input.Items.Count == 0)
            return (null, "A venda precisa ter pelo menos um item.");

        if (string.IsNullOrWhiteSpace(input.CustomerName))
            return (null, "O nome do cliente é obrigatório.");

        if (!Enum.TryParse<PaymentMethod>(input.PaymentMethod, ignoreCase: true, out var paymentMethod))
            return (null, "Forma de pagamento inválida.");

        if (input.Installments < 1)
            return (null, "O número de parcelas precisa ser pelo menos 1.");

        // Agrupa quantidades caso o mesmo produto apareça duas vezes na lista
        var grouped = input.Items
            .GroupBy(i => i.InstrumentId)
            .Select(g => new SaleItemInputDto(g.Key, g.Sum(i => i.Quantity)))
            .ToList();

        var ids = grouped.Select(i => i.InstrumentId).ToList();
        var instruments = await context.Instruments.Where(i => ids.Contains(i.Id)).ToListAsync();

        var sale = new Sale
        {
            CustomerId = input.CustomerId,
            CustomerNameSnapshot = input.CustomerName.Trim(),
            CustomerEmailSnapshot = input.CustomerEmail,
            CustomerPhoneSnapshot = input.CustomerPhone,
            CustomerAddressSnapshot = input.CustomerAddress,
            PaymentMethod = paymentMethod,
            Installments = input.Installments,
            Status = SaleStatus.Pago,
            Origin = input.Origin,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0;

        foreach (var line in grouped)
        {
            if (line.Quantity <= 0)
                return (null, "A quantidade de cada item precisa ser maior que zero.");

            var instrument = instruments.FirstOrDefault(i => i.Id == line.InstrumentId);
            if (instrument == null)
                return (null, $"Produto #{line.InstrumentId} não encontrado.");

            if (instrument.Price == null)
                return (null, $"O produto \"{instrument.Name}\" está com preço sob consulta e não pode ser vendido pelo checkout automático.");

            if (instrument.StockQuantity < line.Quantity)
                return (null, $"Estoque insuficiente para \"{instrument.Name}\" (disponível: {instrument.StockQuantity}).");

            var unitPrice = instrument.Price.Value;
            var subtotal = unitPrice * line.Quantity;
            total += subtotal;

            instrument.StockQuantity -= line.Quantity;

            sale.Items.Add(new SaleItem
            {
                InstrumentId = instrument.Id,
                ProductNameSnapshot = instrument.Name,
                UnitPrice = unitPrice,
                Quantity = line.Quantity,
                Subtotal = subtotal
            });
        }

        sale.Total = total;
        return (sale, null);
    }
}
