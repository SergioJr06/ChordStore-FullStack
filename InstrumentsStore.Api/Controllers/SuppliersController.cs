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

// este codigo define um controller de API RESTful para gerenciar fornecedores (suppliers) no contexto de uma aplicação ASP.NET Core.
// Ele utiliza Entity Framework Core para interagir com o banco de dados e implementa operações CRUD (Create, Read, Update, Delete) para a entidade Supplier.
// O controller está protegido por autenticação, exigindo que o usuário esteja autorizado para acessar os endpoints.

[Authorize]
[ApiController]
[Route("api/admin/suppliers")]
public class SuppliersController : ControllerBase
{
    // Contexto do EF Core injetado via Dependency Injection (IoC container do ASP.NET Core).
    // O ciclo de vida padrão do DbContext é Scoped (uma instância por requisição HTTP).
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context) => _context = context;

    /// Recupera a lista completa de fornecedores ordenados alfabeticamente por nome.
    [HttpGet]
    public async Task>> GetAll()
    {
        // Include(s => s.Instruments): Aplica Eager Loading para carregar o relacionamento 1:N via JOIN no SQL gerado.
        // ToListAsync(): Executa a query de forma assíncrona, liberando a thread do threadpool durante o I/O de rede com o SGBD.
        var suppliers = await _context.Suppliers
            .Include(s => s.Instruments)
            .OrderBy(s => s.Name)
            .ToListAsync();

        // Projeção em memória de entidades de domínio para DTOs, desacoplando o modelo relacional do payload retornado.
        return Ok(suppliers.Select(SupplierDto.From));
    }

    /// Busca um fornecedor específico por chave primária (ID).
    [HttpGet("{id:int}")]
    public async Task> GetById(int id)
    {
        // FirstOrDefaultAsync: Envia LIMIT/TOP 1 ao banco de dados com filtro por PK.
        var supplier = await _context.Suppliers
            .Include(s => s.Instruments)
            .FirstOrDefaultAsync(s => s.Id == id);

        // Tratamento de ausência de recurso: retorna HTTP 404 Not Found conforme especificação REST.
        if (supplier == null) return NotFound();

        return Ok(SupplierDto.From(supplier));
    }

    /// Persiste um novo fornecedor no banco de dados.
    [HttpPost]
    public async Task> Create([FromBody] SupplierSaveDto dto)
    {
        // Validação defensiva de regra de negócio a nível de aplicação.
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do fornecedor é obrigatório." });

        // Instanciação da entidade de domínio a partir dos dados sanitizados do DTO.
        var supplier = new Supplier
        {
            Name = dto.Name.Trim(),
            Document = dto.Document,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address
        };

        // Adiciona a entidade ao Change Tracker com estado 'Added'.
        _context.Suppliers.Add(supplier);

        // Emite o comando SQL INSERT transacionado e atualiza a propriedade Id gerada pela SEQUENCE/IDENTITY do banco.
        await _context.SaveChangesAsync();

        // Retorna HTTP 201 Created com o cabeçalho 'Location' apontando para o endpoint GetById do recurso recém-criado.
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, SupplierDto.From(supplier));
    }

    /// Atualiza os dados de um fornecedor existente.
    [HttpPut("{id:int}")]
    public async Task> Update(int id, [FromBody] SupplierSaveDto dto)
    {
        // Busca a entidade para vinculá-la ao Change Tracker do EF Core.
        var supplier = await _context.Suppliers
            .Include(s => s.Instruments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "O nome do fornecedor é obrigatório." });

        // Mutação de estado: o Change Tracker detecta as propriedades alteradas para gerar um SQL UPDATE otimizado.
        supplier.Name = dto.Name.Trim();
        supplier.Document = dto.Document;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.Address = dto.Address;

        await _context.SaveChangesAsync();

        return Ok(SupplierDto.From(supplier));
    }

    /// Remove um fornecedor garantindo integridade referencial em nível de aplicação.
    [HttpDelete("{id:int}")]
    public async Task Delete(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Instruments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return NotFound();

        // Verificação defensiva de restrição de chave estrangeira (FK) em memória antes da deleção, 
        // evitando exceções brutas de constraint violation no banco de dados.
        if (supplier.Instruments.Count > 0)
            return BadRequest(new { message = "Não é possível excluir: existem produtos vinculados a este fornecedor." });

        // Marca a entidade como 'Deleted' no Change Tracker.
        _context.Suppliers.Remove(supplier);

        // Executa o comando SQL DELETE.
        await _context.SaveChangesAsync();

        // HTTP 204 No Content: Operação idempotente concluída com sucesso sem payload no corpo da resposta.
        return NoContent();
    }
}