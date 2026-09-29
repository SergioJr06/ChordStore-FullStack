using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Este código define um controlador de administração para gerenciar categorias em uma API. Ele permite que usuários autenticados realizem operações CRUD
// (Create, Read, Update, Delete) em categorias, incluindo a verificação de vínculos com produtos antes da exclusão.

namespace InstrumentsStore.Api.Controllers; 

[Authorize] // Apenas usuários autenticados podem acessar este controlador
[ApiController] // Indica que este é um controlador de API
[Route("api/admin/categories")]
public class CategoriesController : ControllerBase // Controlador base para APIs, não retorna Views
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll() // Define que este método responde a requisições GET e retorna uma lista de categorias
    {
        var categories = await _context.Categories
            .Include(c => c.Instruments)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories.Select(CategoryDto.From)); // Retorna a lista de categorias convertida para DTOs
    }

    [HttpGet("{id:int}")] // Define que este método responde a requisições GET com um parâmetro inteiro na rota
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id); // Acessa a tabela de categorias no banco de dados e inclui os instrumentos relacionados
        if (category == null) return NotFound();
        return Ok(CategoryDto.From(category));
    } // Define que este método responde a requisições GET com um parâmetro inteiro na rota

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategorySaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) // Verifica se o nome da categoria é nulo, vazio ou contém apenas espaços em branco
            return BadRequest(new { message = "O nome da categoria é obrigatório." });

        var category = new Category { Name = dto.Name.Trim(), Description = dto.Description }; // Cria uma nova instância da categoria com o nome e descrição fornecidos, removendo espaços em branco do nome
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, CategoryDto.From(category)); // Retorna a categoria criada com o status HTTP 201 (Created) e inclui a localização da nova categoria no cabeçalho da resposta
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategorySaveDto dto) // Define que este método responde a requisições PUT com um parâmetro inteiro na rota e espera um objeto JSON no corpo da requisição
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name)) // Verifica se o nome da categoria é nulo, vazio ou contém apenas espaços em branco
            return BadRequest(new { message = "O nome da categoria é obrigatório." });

        category.Name = dto.Name.Trim();
        category.Description = dto.Description; // Atualiza a descrição da categoria com o valor fornecido no DTO
        await _context.SaveChangesAsync(); // Salva as alterações no banco de dados de forma assíncrona

        return Ok(CategoryDto.From(category));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) // Define que este método responde a requisições DELETE com um parâmetro inteiro na rota
    {
        var category = await _context.Categories.Include(c => c.Instruments).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        if (category.Instruments.Count > 0) // Verifica se existem produtos vinculados a esta categoria antes de permitir a exclusão
            return BadRequest(new { message = "Não é possível excluir: existem produtos vinculados a esta categoria." });

        _context.Categories.Remove(category); // Remove a categoria do contexto do banco de dados
        await _context.SaveChangesAsync();
        return NoContent(); // Retorna um status HTTP 204 (No Content) indicando que a exclusão foi bem-sucedida, mas não há conteúdo para retornar
    }
}
