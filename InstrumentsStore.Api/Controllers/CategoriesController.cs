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
[Route("api/admin/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _context.Categories
            .Include(c => c.Instruments)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories.Select(CategoryDto.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        return Ok(CategoryDto.From(category));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategorySaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome da categoria é obrigatório." });

        var category = new Category { Name = dto.Name.Trim(), Description = dto.Description };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, CategoryDto.From(category));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategorySaveDto dto)
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome da categoria é obrigatório." });

        category.Name = dto.Name.Trim();
        category.Description = dto.Description;
        await _context.SaveChangesAsync();

        return Ok(CategoryDto.From(category));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        if (category.Instruments.Count > 0)
            return BadRequest(new { message = "Não é possível excluir: existem produtos vinculados a esta categoria." });

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
