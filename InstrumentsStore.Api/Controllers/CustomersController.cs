using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// este código define um controlador de administração para gerenciar clientes em uma API.
// Ele permite que usuários autenticados realizem operações CRUD (Create, Read, Update, Delete) em clientes, incluindo filtragem por nome, email ou documento.

namespace InstrumentsStore.Api.Controllers; // Define o namespace do controlador

[Authorize] // Apenas usuários autenticados podem acessar este controlador
[ApiController] // Indica que este é um controlador de API
[Route("api/admin/customers")]
public class CustomersController : ControllerBase // Controlador base para APIs, não retorna Views
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context) => _context = context; // Injeta o contexto do banco de dados no controlador

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll([FromQuery] string? search) // Define que este método responde a requisições GET e permite um parâmetro de consulta opcional para pesquisa
    {
        var query = _context.Customers.Include(c => c.Sales).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Document != null && c.Document.Contains(term)));
        } // Filtra os clientes pelo nome, email ou documento, se o parâmetro de pesquisa for fornecido

        var customers = await query.OrderBy(c => c.Name).ToListAsync(); 
        return Ok(customers.Select(CustomerDto.From)); // Retorna a lista de clientes ordenada por nome, convertida para DTOs
    }

    [HttpGet("{id:int}")] // Define que este método responde a requisições GET com um parâmetro inteiro na rota
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id); // Acessa a tabela de clientes no banco de dados e busca o cliente pelo ID, incluindo suas vendas
        if (customer == null) return NotFound();
        return Ok(CustomerDto.From(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerSaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do cliente é obrigatório." }); // Valida se o nome do cliente foi fornecido, caso contrário retorna um erro de requisição inválida

        var customer = new Customer // Cria um novo objeto Customer com os dados fornecidos no DTO
        {
            Name = dto.Name.Trim(),
            Email = dto.Email,
            Phone = dto.Phone,
            Document = dto.Document,
            Address = dto.Address
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(); // Salva o novo cliente no banco de dados de forma assíncrona

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomerDto.From(customer));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, [FromBody] CustomerSaveDto dto) // Define que este método responde a requisições PUT com um parâmetro inteiro na rota e espera um objeto JSON no corpo da requisição
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do cliente é obrigatório." }); // Valida se o nome do cliente foi fornecido, caso contrário retorna um erro de requisição inválida

        customer.Name = dto.Name.Trim();
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Document = dto.Document;
        customer.Address = dto.Address;
        await _context.SaveChangesAsync();

        return Ok(CustomerDto.From(customer));
    }

    [HttpDelete("{id:int}")] // Define que este método responde a requisições DELETE com um parâmetro inteiro na rota
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.Include(c => c.Sales).FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();

        if (customer.Sales.Count > 0)
            return BadRequest(new { message = "Não é possível excluir: este cliente já tem vendas registradas." }); // Verifica se o cliente possui vendas registradas; se sim, retorna um erro de requisição inválida

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(); // Salva as alterações no banco de dados de forma assíncrona
        return NoContent();
    }
}
