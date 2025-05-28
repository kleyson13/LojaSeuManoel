using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoSeuManoel.Api.Controllers;

[Route("api/pedidos")] // Define a rota base para este controller: http://localhost:porta/api/pedidos
[ApiController]      // Atributo que habilita funcionalidades úteis para APIs
public class PedidosController : ControllerBase
{
    private readonly IPackingService _packingService;

    // Injeção de Dependência: O ASP.NET Core vai fornecer uma instância de IPackingService
    public PedidosController(IPackingService packingService)
    {
        _packingService = packingService;
    }

    // POST api/pedidos
    [HttpPost] // Este método vai responder a requisições HTTP POST
    public async Task<IActionResult> ProcessarPedidos([FromBody] List<PedidoRequestDto> pedidos)
    {
        // [FromBody] indica que os dados do pedido virão no corpo (body) da requisição HTTP, em formato JSON.

        if (pedidos == null || !pedidos.Any())
        {
            return BadRequest("A lista de pedidos não pode ser vazia.");
        }
        if (pedidos.Any(p => p.Produtos == null || !p.Produtos.Any()))
        {
            return BadRequest("Cada pedido deve conter ao menos um produto.");
        }

        try
        {
            var resultado = await _packingService.ProcessarPedidosAsync(pedidos);
            return Ok(resultado); // Retorna HTTP 200 OK com os dados do resultado no corpo da resposta
        }
        catch (ArgumentException ex) // Exceção que definimos em Dimensoes, por exemplo
        {
            return BadRequest($"Erro de validação: {ex.Message}");
        }
        catch (InvalidOperationException ex) // Exceção que podemos ter lançado no algoritmo
        {
            return BadRequest($"Erro no processamento: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Logar o erro ex (não faremos o log completo aqui para simplificar)
            // Em um sistema real, você usaria um sistema de logging (Serilog, NLog, etc.)
            Console.WriteLine($"Erro inesperado: {ex.ToString()}");
            return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação."); // HTTP 500 Internal Server Error
        }
    }
}