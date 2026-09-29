namespace InstrumentsStore.Api.Models;
// Modelo de dados para instrumentos musicais
// Este modelo representa os instrumentos que serão vendidos na loja online.
public class Instrument
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty; // Identificador para URL (ex: "guitarra-fender-stratocaster")
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty; // Espera "novo", "exclusivo" ou "promocao"

    // Preços anuláveis (nullable) para lidar com o "Preço sob consulta"
    public decimal? Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int Installments { get; set; } = 12;

    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? GalleryUrls { get; set; } // Salvaremos no banco como string separada por ";"

    // Controle interno de estoque
    public string Brand { get; set; } = string.Empty;
    public int StockQuantity { get; set; }

    // Categoria e Fornecedor agora são cadastros próprios (Categorias/Fornecedores no admin)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
