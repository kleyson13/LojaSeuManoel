using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Domain.Entities; 
using LojaDoSeuManoel.Domain.Services;
using LojaDoSeuManoel.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Application.Services;

public class PackingService : IPackingService
{
    private readonly AlgoritmoEmpacotamento _algoritmoEmpacotamento;

    // O AlgoritmoEmpacotamento será injetado aqui (veremos Injeção de Dependência depois)
    // Por enquanto, vamos instanciar diretamente para simplificar o exemplo inicial.
    public PackingService() // Mais tarde, injetaremos: public PackingService(AlgoritmoEmpacotamento algoritmo)
    {
        _algoritmoEmpacotamento = new AlgoritmoEmpacotamento();
    }

    public async Task<List<PedidoProcessadoDto>> ProcessarPedidosAsync(List<PedidoRequestDto> pedidosDto)
    {
        var resultadosFinais = new List<PedidoProcessadoDto>();

        foreach (var pedidoDto in pedidosDto)
        {
            // 1. Mapear Produtos DTO para Entidades de Domínio Produto
            var produtosDoPedidoDomain = pedidoDto.Produtos.Select(pDto =>
                new Produto(
                    new Dimensoes(pDto.Altura, pDto.Largura, pDto.Comprimento),
                    pDto.ProdutoId
                )).ToList();

            // 2. Chamar o algoritmo de empacotamento do domínio
            List<CaixaEmbalada> caixasEmbaladasDomain = _algoritmoEmpacotamento.EmbalarPedido(produtosDoPedidoDomain);

            // 3. Mapear o resultado (CaixasEmbaladas do Domínio) para DTOs de resposta
            var pedidoProcessadoResultadoDto = new PedidoProcessadoDto
            {
                PedidoId = pedidoDto.PedidoId,
                Pacotes = caixasEmbaladasDomain.Select(caixaDomain => new CaixaUtilizadaDto
                {
                    NomeCaixa = caixaDomain.TipoCaixaUsada.Nome,
                    AlturaCaixa = caixaDomain.TipoCaixaUsada.Dimensoes.AlturaCm,
                    LarguraCaixa = caixaDomain.TipoCaixaUsada.Dimensoes.LarguraCm,
                    ComprimentoCaixa = caixaDomain.TipoCaixaUsada.Dimensoes.ComprimentoCm,
                    ProdutosNaCaixa = caixaDomain.Produtos.Select(prodDomain => new ProdutoDto
                    {
                        ProdutoId = prodDomain.ProdutoId,
                        Altura = prodDomain.Dimensoes.AlturaCm,
                        Largura = prodDomain.Dimensoes.LarguraCm,
                        Comprimento = prodDomain.Dimensoes.ComprimentoCm
                    }).ToList()
                }).ToList()
            };
            resultadosFinais.Add(pedidoProcessadoResultadoDto);
        }

        // Simula uma operação assíncrona, comum em I/O (como salvar no banco)
        // No nosso caso, o processamento é todo em memória, mas é boa prática usar async/await
        // se houver qualquer chance de operações demoradas ou I/O.
        return await Task.FromResult(resultadosFinais);
    }
}