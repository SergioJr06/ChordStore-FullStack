using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Services;

// utilizado pelo controller de checkout para validar estoque, montar a venda e baixar o estoque

public static class SaleBuilder // static porque não precisa de estado, só de um método utilitário
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

    public static async Task<(Sale? Sale, string? Error)> BuildAsync(AppDbContext context, Input input) // retorna a venda pronta ou uma mensagem de erro
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

        var ids = grouped.Select(i => i.InstrumentId).ToList(); // lista de ids dos instrumentos que estão na venda
        var instruments = await context.Instruments.Where(i => ids.Contains(i.Id)).ToListAsync();

        var sale = new Sale // monta a venda com os dados do cliente e do pagamento
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

        foreach (var line in grouped) // para cada item da venda, valida o estoque e monta os itens da venda
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

            var unitPrice = instrument.Price.Value; // pega o preço do instrumento, que já foi validado para não ser nulo
            var subtotal = unitPrice * line.Quantity; // calcula o subtotal do item
            total += subtotal; // acumula o total da venda

            instrument.StockQuantity -= line.Quantity;

            sale.Items.Add(new SaleItem // adiciona o item à venda, com o preço e quantidade corretos
            {
                InstrumentId = instrument.Id,
                ProductNameSnapshot = instrument.Name,
                UnitPrice = unitPrice,
                Quantity = line.Quantity,
                Subtotal = subtotal
            });
        }

        sale.Total = total; // define o total da venda
        return (sale, null); // retorna a venda pronta e sem erro
    }
}
