using System.Collections.Generic;

namespace InstrumentsStore.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Instrument> Instruments { get; set; } = new List<Instrument>();
}
