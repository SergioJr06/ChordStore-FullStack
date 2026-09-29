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

// este código define um controller de API RESTful para gerenciar vendas (sales) no contexto de uma aplicação ASP.NET Core. Ele utiliza Entity Framework Core para interagir com o banco de dados e implementa operações CRUD (Create, Read, Update) para a entidade Sale.
// O controller está protegido por autenticação, exigindo que o usuário esteja autorizado para acessar os endpoints.

namespace InstrumentsStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/admin/sales")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context) => _context = context;

    [HttpGet] // Recupera a lista completa de vendas, com filtros opcionais por status e intervalo de datas.
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll(
        [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _context.Sales.Include(s => s.Items).AsQueryable(); // Cria uma query base para buscar vendas, incluindo os itens relacionados.

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SaleStatus>(status, true, out var parsedStatus))
            query = query.Where(s => s.Status == parsedStatus);

        if (from.HasValue) query = query.Where(s => s.CreatedAt >= from.Value); // Aplica filtro de data inicial, se fornecida.
        if (to.HasValue) query = query.Where(s => s.CreatedAt <= to.Value);

        var sales = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
        return Ok(sales.Select(SaleDto.From)); // Projeta as entidades de domínio para DTOs antes de retornar a resposta.
    }

    [HttpGet("{id:int}")] // Busca uma venda específica por chave primária (ID).
    public async Task<ActionResult<SaleDto>> GetById(int id)
    {
        var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        return Ok(SaleDto.From(sale));
    }

    // Persiste uma nova venda no banco de dados.
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

        var input = new SaleBuilder.Input( // Cria um objeto de entrada para o SaleBuilder, encapsulando os dados necessários para construir a venda.
            dto.CustomerId, customerName, customerEmail, customerPhone, null,
            dto.PaymentMethod, dto.Installments, dto.Items, Origin: "admin");

        var (sale, error) = await SaleBuilder.BuildAsync(_context, input);
        if (error != null) return BadRequest(new { message = error });

        _context.Sales.Add(sale!);
        await _context.SaveChangesAsync(); // Persiste a nova venda no banco de dados.

        return CreatedAtAction(nameof(GetById), new { id = sale!.Id }, SaleDto.From(sale));
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<SaleDto>> UpdateStatus(int id, [FromBody] SaleStatusUpdateDto dto)
    {
        var sale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound(); // Retorna HTTP 404 Not Found se a venda não for encontrada.

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
        await _context.SaveChangesAsync(); // Persiste a alteração de status no banco de dados.

        return Ok(SaleDto.From(sale));
    }
}
