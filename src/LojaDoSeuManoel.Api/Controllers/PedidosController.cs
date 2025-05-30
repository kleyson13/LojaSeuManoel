using Microsoft.AspNetCore.Mvc;
using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace LojaDoSeuManoel.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPackingService _packingService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPackingService packingService, ILogger<PedidosController> logger)
    {
        _packingService = packingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessarPedidosRaiz([FromBody] ListaPedidosRequestDto request)
    {
        if (request == null || request.Pedidos == null || !request.Pedidos.Any())
        {
            _logger.LogWarning("Tentativa de processar uma lista de pedidos vazia ou nula.");
            return BadRequest("A lista de pedidos não pode ser vazia.");
        }
        if (request.Pedidos.Any(p => p.Produtos == null || !p.Produtos.Any()))
        {
            _logger.LogWarning("Tentativa de processar um pedido individual sem produtos.");
            return BadRequest("Cada pedido individual deve conter ao menos um produto.");
        }
        foreach (var pedido in request.Pedidos)
        {
            foreach (var produto in pedido.Produtos)
            {
                if (produto.Dimensoes.Altura <= 0 || produto.Dimensoes.Largura <= 0 || produto.Dimensoes.Comprimento <= 0)
                {
                    _logger.LogWarning("Produto com dimensões inválidas (não positivas) recebido: {ProdutoId}", produto.ProdutoId);
                    return BadRequest($"Produto '{produto.ProdutoId}' no pedido '{pedido.PedidoId}' possui dimensões inválidas. Todas as dimensões devem ser positivas.");
                }
            }
        }


        try
        {
            List<PedidoProcessadoResponseDto> resultadoProcessamento =
                await _packingService.ProcessarListaDePedidosAsync(request.Pedidos);

            var response = new ListaPedidosResponseDto { Pedidos = resultadoProcessamento };
            _logger.LogInformation("Processamento de {Count} pedidos concluído com sucesso.", request.Pedidos.Count);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de argumento durante o processamento de pedidos.");
            return BadRequest($"Erro nos dados fornecidos: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operação inválida durante o processamento de pedidos.");
            return StatusCode(500, $"Erro de operação: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o processamento de pedidos.");
            return StatusCode(500, "Ocorreu um erro interno inesperado ao processar sua solicitação.");
        }
    }
}