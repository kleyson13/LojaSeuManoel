using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Services;
using LojaDoSeuManoel.Domain.ValueObjects;
using LojaDoSeuManoel.Domain.Results;
using LojaDoSeuManoel.Infrastructure.Data;
using LojaDoSeuManoel.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Application.Services;

public class PackingService : IPackingService
{
    private readonly AlgoritmoEmpacotamento _algoritmoEmpacotamento;
    private readonly AppDbContext _dbContext;

    public PackingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        var tiposDeCaixaDomain = _dbContext.TiposDeCaixa
            .Select(tc => new DefinicaoCaixa(tc.Nome, new Dimensoes(tc.AlturaCm, tc.LarguraCm, tc.ComprimentoCm)))
            .ToList();
        _algoritmoEmpacotamento = new AlgoritmoEmpacotamento(tiposDeCaixaDomain);
    }

    public async Task<List<PedidoProcessadoResponseDto>> ProcessarListaDePedidosAsync(List<PedidoRequestDto> pedidosRequest)
    {
        var resultadosFinaisDto = new List<PedidoProcessadoResponseDto>();

        foreach (var pedidoDto in pedidosRequest)
        {
            var produtosDoPedidoDomain = pedidoDto.Produtos.Select(pDto =>
                new Produto(
                    new Dimensoes(pDto.Dimensoes.Altura, pDto.Dimensoes.Largura, pDto.Dimensoes.Comprimento),
                    pDto.ProdutoId
                )).ToList();

            ResultadoEmpacotamentoPedido resultadoAlgoritmo = _algoritmoEmpacotamento.EmbalarPedido(produtosDoPedidoDomain);

            var pedidoPersistencia = new PedidoProcessadoPersistencia
            {
                Id = Guid.NewGuid(),
                PedidoOriginalId = pedidoDto.PedidoId.ToString(),
                DataRecepcao = DateTime.UtcNow
            };

            foreach (var caixaDomain in resultadoAlgoritmo.CaixasUtilizadas)
            {
                var caixaUtilizadaPersistencia = new CaixaUtilizadaPersistencia
                {
                    Id = Guid.NewGuid(),
                    PedidoProcessadoId = pedidoPersistencia.Id,
                    NomeCaixa = caixaDomain.TipoCaixaUsada.Nome,
                    AlturaCaixaCm = caixaDomain.TipoCaixaUsada.Dimensoes.AlturaCm,
                    LarguraCaixaCm = caixaDomain.TipoCaixaUsada.Dimensoes.LarguraCm,
                    ComprimentoCaixaCm = caixaDomain.TipoCaixaUsada.Dimensoes.ComprimentoCm,
                };
                foreach (var produtoDomain in caixaDomain.Produtos)
                {
                    caixaUtilizadaPersistencia.ProdutosNaCaixa.Add(new ProdutoEmCaixaPersistencia
                    {
                        Id = Guid.NewGuid(),
                        CaixaUtilizadaId = caixaUtilizadaPersistencia.Id,
                        ProdutoOriginalId = produtoDomain.ProdutoId,
                        AlturaCm = produtoDomain.Dimensoes.AlturaCm,
                        LarguraCm = produtoDomain.Dimensoes.LarguraCm,
                        ComprimentoCm = produtoDomain.Dimensoes.ComprimentoCm
                    });
                }
                pedidoPersistencia.CaixasUtilizadas.Add(caixaUtilizadaPersistencia);
            }
            
             _dbContext.PedidosProcessados.Add(pedidoPersistencia);

            var pedidoProcessadoResultadoDto = new PedidoProcessadoResponseDto
            {
                PedidoId = pedidoDto.PedidoId,
                Caixas = resultadoAlgoritmo.CaixasUtilizadas.Select(caixaDomain => new CaixaEmpacotadaResponseDto
                {
                    CaixaId = caixaDomain.TipoCaixaUsada.Nome,
                    Produtos = caixaDomain.Produtos.Select(p => p.ProdutoId ?? "ID_DESCONHECIDO").ToList()
                }).ToList()
            };

            if (resultadoAlgoritmo.ProdutosNaoEmbalados.Any())
            {
                var produtosNaoEmbaladosIds = resultadoAlgoritmo.ProdutosNaoEmbalados
                                                .Select(p => p.ProdutoId ?? "ID_DESCONHECIDO")
                                                .ToList();
                
                if(produtosNaoEmbaladosIds.Any()){
                    pedidoProcessadoResultadoDto.Caixas.Add(new CaixaEmpacotadaResponseDto
                    {
                        CaixaId = null,
                        Produtos = produtosNaoEmbaladosIds,
                        Observacao = "Produto(s) não cabe(m) em nenhuma caixa disponível."
                    });
                }
            }
            resultadosFinaisDto.Add(pedidoProcessadoResultadoDto);
        }

        await _dbContext.SaveChangesAsync();
        return resultadosFinaisDto;
    }
}