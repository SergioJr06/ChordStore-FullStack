using System;
using System.Collections.Generic;

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

    /// <summary>Origem da venda: "admin" (lançada manualmente) ou "loja" (checkout do site).</summary>
    public string Origin { get; set; } = "admin";

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
