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
[Route("api/admin/customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Customers.Include(c => c.Sales).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Document != null && c.Document.Contains(term)));
        }

        var customers = await query.OrderBy(c => c.Name).ToListAsync();
        return Ok(customers.Select(CustomerDto.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        return Ok(CustomerDto.From(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerSaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do cliente é obrigatório." });

        var customer = new Customer
        {
            Name = dto.Name.Trim(),
            Email = dto.Email,
            Phone = dto.Phone,
            Document = dto.Document,
            Address = dto.Address
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomerDto.From(customer));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, [FromBody] CustomerSaveDto dto)
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do cliente é obrigatório." });

        customer.Name = dto.Name.Trim();
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Document = dto.Document;
        customer.Address = dto.Address;
        await _context.SaveChangesAsync();

        return Ok(CustomerDto.From(customer));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();

        if (customer.Sales.Count > 0)
            return BadRequest(new { message = "Não é possível excluir: este cliente já tem vendas registradas." });

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
