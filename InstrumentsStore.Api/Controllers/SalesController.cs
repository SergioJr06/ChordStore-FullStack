using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using InstrumentsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/admin/sales")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll(
        [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _context.Sales.Include(s => s.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SaleStatus>(status, true, out var parsedStatus))
            query = query.Where(s => s.Status == parsedStatus);

        if (from.HasValue) query = query.Where(s => s.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(s => s.CreatedAt <= to.Value);

        var sales = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
        return Ok(sales.Select(SaleDto.From));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SaleDto>> GetById(int id)
    {
        var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        return Ok(SaleDto.From(sale));
    }

    /// <summary>Lança uma venda manualmente (tela tipo PDV do admin).</summary>
    [HttpPost]
    public async Task<ActionResult<SaleDto>> Create([FromBody] SaleCreateDto dto)
    {
        string customerName = dto.CustomerName ?? "";
        string? customerEmail = dto.CustomerEmail;
        string? customerPhone = dto.CustomerPhone;

        if (dto.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId.Value);
            if (customer == null) return BadRequest(new { message = "Cliente informado não existe." });
            customerName = customer.Name;
            customerEmail ??= customer.Email;
            customerPhone ??= customer.Phone;
        }

        var input = new SaleBuilder.Input(
            dto.CustomerId, customerName, customerEmail, customerPhone, null,
            dto.PaymentMethod, dto.Installments, dto.Items, Origin: "admin");

        var (sale, error) = await SaleBuilder.BuildAsync(_context, input);
        if (error != null) return BadRequest(new { message = error });

        _context.Sales.Add(sale!);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sale!.Id }, SaleDto.From(sale));
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<SaleDto>> UpdateStatus(int id, [FromBody] SaleStatusUpdateDto dto)
    {
        var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();

        if (!Enum.TryParse<SaleStatus>(dto.Status, true, out var status))
            return BadRequest(new { message = "Status inválido. Use Pendente, Pago ou Cancelado." });

        // Se cancelar uma venda que estava paga, devolve o estoque.
        if (status == SaleStatus.Cancelado && sale.Status != SaleStatus.Cancelado)
        {
            foreach (var item in sale.Items)
            {
                var instrument = await _context.Instruments.FindAsync(item.InstrumentId);
                if (instrument != null) instrument.StockQuantity += item.Quantity;
            }
        }

        sale.Status = status;
        await _context.SaveChangesAsync();

        return Ok(SaleDto.From(sale));
    }
}
