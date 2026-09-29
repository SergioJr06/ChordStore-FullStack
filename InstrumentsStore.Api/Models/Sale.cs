using System;
using System.Collections.Generic;
//codigo gerado para a classe Sale, que representa uma venda no sistema de loja de instrumentos musicais. A classe contém propriedades para armazenar informações sobre o cliente, status da venda, método de pagamento, parcelas, total da venda e origem da venda.
//Além disso, possui uma coleção de itens vendidos (SaleItem) associados à venda.
namespace InstrumentsStore.Api.Models;

public class Sale
{
    public int Id { get; set; }

    // Venda pode ser feita para um cliente cadastrado (CustomerId) ou avulsa,
    // usando os dados soltos abaixo (ex.: cliente que comprou no site sem cadastro prévio).
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string CustomerNameSnapshot { get; set; } = string.Empty;
    public string? CustomerEmailSnapshot { get; set; }
    public string? CustomerPhoneSnapshot { get; set; }
    public string? CustomerAddressSnapshot { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public SaleStatus Status { get; set; } = SaleStatus.Pendente;
    public PaymentMethod PaymentMethod { get; set; }
    public int Installments { get; set; } = 1;
    public decimal Total { get; set; }

    // Origem da venda: "admin" (venda feita pelo administrador) ou "site" (venda feita pelo cliente no site)
    public string Origin { get; set; } = "admin";

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
