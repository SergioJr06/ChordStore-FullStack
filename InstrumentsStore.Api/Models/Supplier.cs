using System.Collections.Generic;

namespace InstrumentsStore.Api.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Document { get; set; } // CNPJ
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();
}
