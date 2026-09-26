using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstrumentDto>>> GetInstruments([FromQuery] string? section)
    {
        var query = _context.Instruments.AsNoTracking();

        // Filtra pela seção se o React solicitar (ex: ?section=novo)
        if (!string.IsNullOrEmpty(section))
        {
            query = query.Where(i => i.Section.ToLower() == section.ToLower());
        }

        var instruments = await query.OrderBy(i => i.Id).ToListAsync();
        return Ok(instruments.Select(InstrumentDto.From));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<InstrumentDto>> GetInstrumentBySlug(string slug)
    {
        var instrument = await _context.Instruments.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Slug == slug);

        if (instrument == null)
            return NotFound();

        return Ok(InstrumentDto.From(instrument));
    }
}