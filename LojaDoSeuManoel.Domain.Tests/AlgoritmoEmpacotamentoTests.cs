using Xunit;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Services;
using LojaDoSeuManoel.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoSeuManoel.Domain.Tests;

public class AlgoritmoEmpacotamentoTests
{
    private readonly List<DefinicaoCaixa> _caixasDisponiveisPadrao;
    private readonly AlgoritmoEmpacotamento _algoritmo;

    public AlgoritmoEmpacotamentoTests()
    {
        _caixasDisponiveisPadrao = new List<DefinicaoCaixa>
        {
            new DefinicaoCaixa("Caixa P", new Dimensoes(10, 10, 10)), // Volume 1000
            new DefinicaoCaixa("Caixa M", new Dimensoes(30, 40, 80)), // Volume 96000
            new DefinicaoCaixa("Caixa G", new Dimensoes(50, 80, 60)), // Volume 240000
        };
        _algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);
    }

    [Fact]
    public void EmbalarPedido_ComListaDeProdutosVazia_DeveRetornarListaDeCaixasVazia()
    {
        var produtos = new List<Produto>();

        var resultado = _algoritmo.EmbalarPedido(produtos);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public void EmbalarPedido_ComUmProdutoPequeno_DeveUsarUmaCaixaPequena()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(5, 5, 5), "PROD001")
        };
        var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);

        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Single(resultado);
        Assert.Equal("Caixa P", resultado.First().TipoCaixaUsada.Nome);
        Assert.Single(resultado.First().Produtos);
        Assert.Equal("PROD001", resultado.First().Produtos.First().ProdutoId);
    }

    [Fact]
    public void EmbalarPedido_ComProdutoQueExigeRotacao_DeveEmbalarCorretamente()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(5, 5, 8), "PROD_ROTA")
        };
         var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);

        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Single(resultado);
        Assert.Equal("Caixa P", resultado.First().TipoCaixaUsada.Nome);
        Assert.Equal("PROD_ROTA", resultado.First().Produtos.First().ProdutoId);
    }

    [Fact]
    public void EmbalarPedido_DoisProdutosPequenos_DevemIrNaMesmaCaixaPequena()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(4, 4, 4), "PROD1"),
            new Produto(new Dimensoes(3, 3, 3), "PROD2")
        };
        var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);

        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Single(resultado);
        Assert.Equal("Caixa P", resultado.First().TipoCaixaUsada.Nome);
        Assert.Equal(2, resultado.First().Produtos.Count);
        Assert.Contains(resultado.First().Produtos, p => p.ProdutoId == "PROD1");
        Assert.Contains(resultado.First().Produtos, p => p.ProdutoId == "PROD2");
    }

    [Fact]
    public void EmbalarPedido_ProdutoMuitoGrandeParaCaixaPequena_DeveUsarCaixaMaior()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(20, 20, 20), "PROD_GRANDE")
        };
        var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);

        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Single(resultado);
        Assert.Equal("Caixa M", resultado.First().TipoCaixaUsada.Nome);
    }

    [Fact]
    public void EmbalarPedido_ProdutosQueExigemMultiplasCaixas_DeveRetornarMultiplasCaixas()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(25, 35, 70), "PROD_G1"),
            new Produto(new Dimensoes(25, 35, 70), "PROD_G2")
        };
        var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);

        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, caixa => Assert.Equal("Caixa M", caixa.TipoCaixaUsada.Nome));
        Assert.Single(resultado[0].Produtos);
        Assert.Single(resultado[1].Produtos);
    }

    [Fact]
    public void EmbalarPedido_ProdutoMaiorQueTodasAsCaixas_DeveRetornarListaVaziaOuLancarExcecao()
    {
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(100, 100, 100), "PROD_GIGANTE")
        };

        var algoritmo = new AlgoritmoEmpacotamento(_caixasDisponiveisPadrao);
        var resultado = algoritmo.EmbalarPedido(produtos);

        Assert.Empty(resultado);
    }
}