namespace InstrumentsStore.Api.Models;
// codigo gerado para representar um item de venda, que referencia a venda e o instrumento vendido
public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale? Sale { get; set; }

    public int InstrumentId { get; set; }
    public Instrument? Instrument { get; set; }

    // Guardamos nome e preço "congelados" no momento da venda, pra não
    // mudar o histórico de vendas antigas se o produto for editado depois.
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
