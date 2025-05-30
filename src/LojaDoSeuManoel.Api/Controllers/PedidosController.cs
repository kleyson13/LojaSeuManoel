using Microsoft.AspNetCore.Mvc;
using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services;
using LojaDoSeuManoel.Api.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Api.Controllers;

[Route("api/pedidos")]
[ApiController]
[ApiKeyAuth]
public class PedidosController : ControllerBase
{
    private readonly IPackingService _packingService;

    public PedidosController(IPackingService packingService)
    {
        _packingService = packingService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessarPedidos([FromBody] List<PedidoRequestDto> pedidos)
    {
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
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Erro de validação: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Erro no processamento: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex.ToString()}");
            return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação.");
        }
    }
}