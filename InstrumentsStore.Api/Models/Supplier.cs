using System.Collections.Generic;
// codigo usado para representar um fornecedor de instrumentos musicais, com informações como nome, documento (CNPJ), telefone, email e endereço.
// Além disso, mantém uma coleção de instrumentos fornecidos por este fornecedor.
namespace InstrumentsStore.Api.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Document { get; set; } // CNPJ
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>(); // Mantém uma coleção de instrumentos fornecidos por este fornecedor.
}
