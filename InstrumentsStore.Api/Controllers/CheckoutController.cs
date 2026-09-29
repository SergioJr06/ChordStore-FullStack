using System.Threading.Tasks;
using InstrumentsStore.Api.Data;
using InstrumentsStore.Api.Dtos;
using InstrumentsStore.Api.Models;
using InstrumentsStore.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstrumentsStore.Api.Controllers;

// este codigo define um controlador de checkout para processar pedidos em uma API. Ele permite que clientes façam pedidos, criando ou reutilizando registros de clientes no banco de dados e registrando a venda.
// O controlador utiliza Entity Framework Core para interagir com o banco de dados e valida os dados recebidos antes de criar a venda.

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase // Controlador base para APIs, não retorna Views
{
    private readonly AppDbContext _context;

    public CheckoutController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<CheckoutResultDto>> Checkout([FromBody] CheckoutDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
            return BadRequest(new { message = "Informe um e-mail para receber a confirmação do pedido." });

        // Verifica se o cliente já existe no banco de dados pelo e-mail fornecido. Se não existir, cria um novo registro de cliente.
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
        if (customer == null)
        {
            customer = new Customer // Cria um novo objeto Customer com os dados fornecidos no DTO
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
            customer.Id, dto.CustomerName, dto.CustomerEmail, dto.CustomerPhone, dto.CustomerAddress, // Cria um objeto de entrada para o construtor de vendas com os dados do cliente e do pedido
            dto.PaymentMethod, dto.Installments, dto.Items, Origin: "loja");

        var (sale, error) = await SaleBuilder.BuildAsync(_context, input);
        if (error != null) return BadRequest(new { message = error }); // Se houver algum erro na construção da venda, retorna um erro 400 com a mensagem de erro.

        _context.Sales.Add(sale!);
        await _context.SaveChangesAsync(); // Salva a venda no banco de dados e retorna um objeto CheckoutResultDto com os detalhes da venda, incluindo o ID, o total e a data de criação.

        return Ok(new CheckoutResultDto(sale!.Id, sale.Total, sale.CreatedAt)); // Retorna um objeto CheckoutResultDto com os detalhes da venda, incluindo o ID, o total e a data de criação.
    }
}
