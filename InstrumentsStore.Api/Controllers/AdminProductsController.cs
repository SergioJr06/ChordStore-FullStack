using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// este código define um controlador de administração para gerenciar produtos (instrumentos) em uma API. Ele permite que usuários autenticados realizem operações CRUD (Create, Read, Update, Delete) em instrumentos, incluindo filtragem por categoria e fornecedor.
// O controlador utiliza Entity Framework Core para interagir com o banco de dados e valida os dados recebidos antes de criar ou atualizar registros.

namespace InstrumentsStore.Api.Controllers;

[Authorize] // Apenas usuários autenticados podem acessar este controlador
[ApiController] // Indica que este é um controlador de API
[Route("api/admin/products")] // Define a rota base para este controlador
public class AdminProductsController : ControllerBase // Controlador base para APIs, não retorna Views
{
    private readonly AppDbContext _context;

    public AdminProductsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminInstrumentDto>>> GetAll(
        [FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] int? supplierId)
    {
        var query = _context.Instruments // Acessa a tabela de instrumentos no banco de dados
            .Include(i => i.Category)
            .Include(i => i.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(i => i.Name.ToLower().Contains(term) || i.Slug.ToLower().Contains(term));
        }

        if (categoryId.HasValue) query = query.Where(i => i.CategoryId == categoryId); // Filtra por categoria, se fornecida
        if (supplierId.HasValue) query = query.Where(i => i.SupplierId == supplierId);

        var items = await query.OrderBy(i => i.Name).ToListAsync(); // Ordena os resultados por nome e executa a consulta de forma assíncrona
        return Ok(items.Select(AdminInstrumentDto.From));
    }

    [HttpGet("{id:int}")] // Define que este método responde a requisições GET com um parâmetro inteiro na rota
    public async Task<ActionResult<AdminInstrumentDto>> GetById(int id)
    {
        var item = await _context.Instruments // Acessa a tabela de instrumentos no banco de dados
            .Include(i => i.Category)
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null) return NotFound();
        return Ok(AdminInstrumentDto.From(item));
    }

    [HttpPost]
    public async Task<ActionResult<AdminInstrumentDto>> Create([FromBody] AdminInstrumentSaveDto dto) // Define que este método responde a requisições POST e espera um objeto JSON no corpo da requisição
    {
        var error = await ValidateAsync(dto, currentId: null);
        if (error != null) return BadRequest(new { message = error });

        var instrument = new Instrument // Cria um novo objeto Instrument com os dados fornecidos no DTO
        {
            Slug = dto.Slug.Trim(),
            Name = dto.Name.Trim(),
            Section = dto.Section.Trim(),
            Price = dto.Price,
            OldPrice = dto.OldPrice,
            Installments = dto.Installments,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            GalleryUrls = dto.GalleryUrlsAsString(),
            Brand = dto.Brand,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId
        };

        _context.Instruments.Add(instrument); // Adiciona o novo instrumento ao contexto do banco de dados
        await _context.SaveChangesAsync();
        await _context.Entry(instrument).Reference(i => i.Category).LoadAsync();
        await _context.Entry(instrument).Reference(i => i.Supplier).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = instrument.Id }, AdminInstrumentDto.From(instrument)); // Retorna um status 201 Created com a localização do novo recurso e os dados do instrumento criado
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AdminInstrumentDto>> Update(int id, [FromBody] AdminInstrumentSaveDto dto) // Define que este método responde a requisições PUT com um parâmetro inteiro na rota e espera um objeto JSON no corpo da requisição
    {
        var instrument = await _context.Instruments.FirstOrDefaultAsync(i => i.Id == id);
        if (instrument == null) return NotFound();

        var error = await ValidateAsync(dto, currentId: id);
        if (error != null) return BadRequest(new { message = error }); // Retorna um status 400 Bad Request com uma mensagem de erro se a validação falhar

        instrument.Slug = dto.Slug.Trim();
        instrument.Name = dto.Name.Trim();
        instrument.Section = dto.Section.Trim();
        instrument.Price = dto.Price;
        instrument.OldPrice = dto.OldPrice;
        instrument.Installments = dto.Installments;
        instrument.Description = dto.Description;
        instrument.ImageUrl = dto.ImageUrl;
        instrument.GalleryUrls = dto.GalleryUrlsAsString();
        instrument.Brand = dto.Brand;
        instrument.StockQuantity = dto.StockQuantity;
        instrument.CategoryId = dto.CategoryId;
        instrument.SupplierId = dto.SupplierId;

        await _context.SaveChangesAsync();
        await _context.Entry(instrument).Reference(i => i.Category).LoadAsync();
        await _context.Entry(instrument).Reference(i => i.Supplier).LoadAsync();

        return Ok(AdminInstrumentDto.From(instrument));
    }

    [HttpDelete("{id:int}")] // Define que este método responde a requisições DELETE com um parâmetro inteiro na rota
    public async Task<IActionResult> Delete(int id)
    {
        var instrument = await _context.Instruments.FirstOrDefaultAsync(i => i.Id == id);
        if (instrument == null) return NotFound();

        var hasSales = await _context.SaleItems.AnyAsync(si => si.InstrumentId == id);
        if (hasSales)
            return BadRequest(new { message = "Não é possível excluir: este produto já possui vendas registradas. Considere zerar o estoque." });

        _context.Instruments.Remove(instrument);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> ValidateAsync(AdminInstrumentSaveDto dto, int? currentId) // Valida os dados do DTO antes de criar ou atualizar um instrumento
    {
        if (string.IsNullOrWhiteSpace(dto.Slug)) return "O slug é obrigatório.";
        if (string.IsNullOrWhiteSpace(dto.Name)) return "O nome é obrigatório.";
        if (string.IsNullOrWhiteSpace(dto.Section)) return "A seção é obrigatória (novo, exclusivo ou promocao).";
        if (dto.StockQuantity < 0) return "O estoque não pode ser negativo.";

        var slugTaken = await _context.Instruments // Verifica se já existe outro instrumento com o mesmo slug, ignorando o instrumento atual (se estiver atualizando)
            .AnyAsync(i => i.Slug == dto.Slug.Trim() && i.Id != (currentId ?? 0));
        if (slugTaken) return "Já existe um produto com esse slug.";

        if (dto.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            return "Categoria informada não existe.";

        if (dto.SupplierId.HasValue && !await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId))
            return "Fornecedor informado não existe.";

        return null;
    }
}
