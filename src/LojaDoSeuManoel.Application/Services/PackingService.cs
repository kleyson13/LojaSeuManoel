using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Services;
using LojaDoSeuManoel.Domain.ValueObjects;
using LojaDoSeuManoel.Infrastructure.Data;
using LojaDoSeuManoel.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<PedidoProcessadoDto>> ProcessarPedidosAsync(List<PedidoRequestDto> pedidosDto)
    {
        var resultadosFinaisDto = new List<PedidoProcessadoDto>();

        foreach (var pedidoDto in pedidosDto)
        {
            var produtosDoPedidoDomain = pedidoDto.Produtos.Select(pDto =>
                new Produto(
                    new Dimensoes(pDto.Altura, pDto.Largura, pDto.Comprimento),
                    pDto.ProdutoId
                )).ToList();

            List<CaixaEmbalada> caixasEmbaladasDomain = _algoritmoEmpacotamento.EmbalarPedido(produtosDoPedidoDomain);

            var pedidoPersistencia = new PedidoProcessadoPersistencia
            {
                Id = Guid.NewGuid(),
                PedidoOriginalId = pedidoDto.PedidoId,
                DataRecepcao = DateTime.UtcNow
            };

            foreach (var caixaDomain in caixasEmbaladasDomain)
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
            resultadosFinaisDto.Add(pedidoProcessadoResultadoDto);
        }

        await _dbContext.SaveChangesAsync();

        return resultadosFinaisDto;
    }
}