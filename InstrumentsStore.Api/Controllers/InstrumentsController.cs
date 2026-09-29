using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// este codigo não requer autenticação, pois é consumido pelo front-end público (React) para exibir instrumentos e detalhes de produtos.
// tem como objetivo fornecer endpoints públicos para listar instrumentos e obter detalhes de um instrumento específico, sem necessidade de login ou token JWT.

namespace InstrumentsStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstrumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InstrumentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet] // Endpoint público para listar instrumentos, com opção de filtrar por seção (ex: ?section=novo).
    public async Task<ActionResult<IEnumerable<InstrumentDto>>> GetInstruments([FromQuery] string? section)
    {
        var query = _context.Instruments.AsNoTracking();

        // Filtra pela seção se o React solicitar (ex: ?section=novo)
        if (!string.IsNullOrEmpty(section))
        {
            query = query.Where(i => i.Section.ToLower() == section.ToLower());
        }

        var instruments = await query.OrderBy(i => i.Id).ToListAsync();
        return Ok(instruments.Select(InstrumentDto.From)); // Projeção de entidades para DTOs, desacoplando o modelo relacional do payload retornado.
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<InstrumentDto>> GetInstrumentBySlug(string slug)
    {
        var instrument = await _context.Instruments.AsNoTracking() // AsNoTracking() otimiza a consulta para leitura, evitando rastreamento de alterações pelo EF Core.
            .FirstOrDefaultAsync(i => i.Slug == slug);

        if (instrument == null)
            return NotFound();

        return Ok(InstrumentDto.From(instrument)); // Projeção de entidade para DTO, desacoplando o modelo relacional do payload retornado.
    }
}