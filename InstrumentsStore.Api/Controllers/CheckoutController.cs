using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using InstrumentsStore.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Controllers;

/// <summary>
/// Endpoint público (sem autenticação) usado pelo carrinho do site do cliente
/// para finalizar a compra. Gera uma venda com origem "loja" e baixa o estoque.
/// </summary>
[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly AppDbContext _context;

    public CheckoutController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<CheckoutResultDto>> Checkout([FromBody] CheckoutDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
            return BadRequest(new { message = "Informe um e-mail para receber a confirmação do pedido." });

        // Tenta reaproveitar um cliente já cadastrado com o mesmo e-mail; se não existir, cria um novo cadastro.
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
        if (customer == null)
        {
            customer = new Customer
            {
                Name = dto.CustomerName,
                Email = dto.CustomerEmail,
                Phone = dto.CustomerPhone,
                Address = dto.CustomerAddress
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(); // gera o Id do cliente antes de vincular na venda
        }

        var input = new SaleBuilder.Input(
            customer.Id, dto.CustomerName, dto.CustomerEmail, dto.CustomerPhone, dto.CustomerAddress,
            dto.PaymentMethod, dto.Installments, dto.Items, Origin: "loja");

        var (sale, error) = await SaleBuilder.BuildAsync(_context, input);
        if (error != null) return BadRequest(new { message = error });

        _context.Sales.Add(sale!);
        await _context.SaveChangesAsync();

        return Ok(new CheckoutResultDto(sale!.Id, sale.Total, sale.CreatedAt));
    }
}
