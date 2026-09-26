using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/admin/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var suppliers = await _context.Suppliers
            .Include(s => s.Instruments)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(suppliers.Select(SupplierDto.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id)
    {
        var supplier = await _context.Suppliers.Include(s => s.Instruments).FirstOrDefaultAsync(s => s.Id == id);
        if (supplier == null) return NotFound();
        return Ok(SupplierDto.From(supplier));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create([FromBody] SupplierSaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do fornecedor é obrigatório." });

        var supplier = new Supplier
        {
            Name = dto.Name.Trim(),
            Document = dto.Document,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, SupplierDto.From(supplier));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, [FromBody] SupplierSaveDto dto)
    {
        var supplier = await _context.Suppliers.Include(s => s.Instruments).FirstOrDefaultAsync(s => s.Id == id);
        if (supplier == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do fornecedor é obrigatório." });

        supplier.Name = dto.Name.Trim();
        supplier.Document = dto.Document;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.Address = dto.Address;
        await _context.SaveChangesAsync();

        return Ok(SupplierDto.From(supplier));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _context.Suppliers.Include(s => s.Instruments).FirstOrDefaultAsync(s => s.Id == id);
        if (supplier == null) return NotFound();

        if (supplier.Instruments.Count > 0)
            return BadRequest(new { message = "Não é possível excluir: existem produtos vinculados a este fornecedor." });

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
