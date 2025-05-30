using Xunit;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Services;
using LojaDoSeuManoel.Domain.ValueObjects;
using LojaDoSeuManoel.Domain.Results;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoSeuManoel.Domain.Tests;

public class AlgoritmoEmpacotamentoTests
{
    private readonly List<DefinicaoCaixa> _caixasEspecificadasPeloDesafio;

    public AlgoritmoEmpacotamentoTests()
    {
        _caixasEspecificadasPeloDesafio = new List<DefinicaoCaixa>
        {
            new DefinicaoCaixa("Caixa 1", new Dimensoes(30, 40, 80)),
            new DefinicaoCaixa("Caixa 2", new Dimensoes(80, 50, 40)),
            new DefinicaoCaixa("Caixa 3", new Dimensoes(50, 80, 60)),
        };
    }

    private AlgoritmoEmpacotamento CriarAlgoritmo()
    {
        var caixasParaTeste = _caixasEspecificadasPeloDesafio
            .Select(c => new DefinicaoCaixa(c.Nome, 
                new Dimensoes(c.Dimensoes.AlturaCm, c.Dimensoes.LarguraCm, c.Dimensoes.ComprimentoCm)))
            .ToList();
        return new AlgoritmoEmpacotamento(caixasParaTeste);
    }

    [Fact]
    public void EmbalarPedido_ComListaDeProdutosVazia_DeveRetornarResultadoVazio()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>();

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);
    }

    [Fact]
    public void EmbalarPedido_ComUmProdutoMuitoPequeno_DeveUsarCaixa1()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(10, 10, 10), "PROD_PEQUENO")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);

        var caixaUsada = resultado.CaixasUtilizadas.First();
        Assert.Equal("Caixa 1", caixaUsada.TipoCaixaUsada.Nome);
        Assert.Single(caixaUsada.Produtos);
        Assert.Equal("PROD_PEQUENO", caixaUsada.Produtos.First().ProdutoId);
    }

    [Fact]
    public void EmbalarPedido_ComProdutoQueExigeRotacaoParaCaberNaCaixa1_DeveEmbalarCorretamente()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(25, 70, 35), "PROD_ROTACAO")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);
        Assert.Equal("Caixa 1", resultado.CaixasUtilizadas.First().TipoCaixaUsada.Nome);
        Assert.Equal("PROD_ROTACAO", resultado.CaixasUtilizadas.First().Produtos.First().ProdutoId);
    }

    [Fact]
    public void EmbalarPedido_DoisProdutosPequenosQueCabemJuntos_DevemIrNaMesmaCaixa1()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(10, 15, 20), "PROD1"),
            new Produto(new Dimensoes(12, 10, 18), "PROD2")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);

        var caixaUsada = resultado.CaixasUtilizadas.First();
        Assert.Equal("Caixa 1", caixaUsada.TipoCaixaUsada.Nome);
        Assert.Equal(2, caixaUsada.Produtos.Count);
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD1");
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD2");
    }

    [Fact]
    public void EmbalarPedido_ProdutoQueNaoCabeCaixa1MasCabeCaixa2_DeveUsarCaixa2_CenarioCorrigido()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(45, 35, 35), "PROD_EXCLUSIVO_CAIXA2")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);
        Assert.Equal("Caixa 2", resultado.CaixasUtilizadas.First().TipoCaixaUsada.Nome);
    }

    [Fact]
    public void EmbalarPedido_DoisProdutosMediosQueExigemDuasCaixas1_DeveUsarDuasCaixas1()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(25, 35, 75), "PROD_M1"),
            new Produto(new Dimensoes(25, 35, 75), "PROD_M2")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.CaixasUtilizadas.Count);
        Assert.Empty(resultado.ProdutosNaoEmbalados);
        Assert.All(resultado.CaixasUtilizadas, caixa => Assert.Equal("Caixa 1", caixa.TipoCaixaUsada.Nome));
        Assert.Single(resultado.CaixasUtilizadas[0].Produtos);
        Assert.Single(resultado.CaixasUtilizadas[1].Produtos);
        var produtoNaCaixa0 = resultado.CaixasUtilizadas[0].Produtos.First().ProdutoId;
        var produtoNaCaixa1 = resultado.CaixasUtilizadas[1].Produtos.First().ProdutoId;
        Assert.NotEqual(produtoNaCaixa0, produtoNaCaixa1);
        Assert.True((produtoNaCaixa0 == "PROD_M1" && produtoNaCaixa1 == "PROD_M2") || (produtoNaCaixa0 == "PROD_M2" && produtoNaCaixa1 == "PROD_M1"));
    }

    [Fact]
    public void EmbalarPedido_ProdutoMaiorQueTodasAsCaixas_DeveRetornarProdutoEmNaoEmbalados()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(90, 90, 90), "PROD_GIGANTE")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado.CaixasUtilizadas);
        Assert.Single(resultado.ProdutosNaoEmbalados);
        Assert.Equal("PROD_GIGANTE", resultado.ProdutosNaoEmbalados.First().ProdutoId);
    }
    
    [Fact]
    public void EmbalarPedido_ComProdutosEmbalaveisEUmNaoEmbalavel_DeveEmbalarOsPossiveisERetornarNaoEmbalavel()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(10, 10, 10), "PROD_OK_1"),
            new Produto(new Dimensoes(90, 90, 90), "PROD_GIGANTE"),
            new Produto(new Dimensoes(12, 12, 12), "PROD_OK_2")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Single(resultado.ProdutosNaoEmbalados);

        Assert.Equal("PROD_GIGANTE", resultado.ProdutosNaoEmbalados.First().ProdutoId);

        var caixaUsada = resultado.CaixasUtilizadas.First();
        Assert.Equal("Caixa 1", caixaUsada.TipoCaixaUsada.Nome);
        Assert.Equal(2, caixaUsada.Produtos.Count);
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD_OK_1");
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD_OK_2");
    }

    [Fact]
    public void EmbalarPedido_TresProdutosQueCabemNaCaixa1_DevemIrNaMesmaCaixa1()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(10, 10, 70), "PROD_A"),
            new Produto(new Dimensoes(10, 10, 70), "PROD_B"),
            new Produto(new Dimensoes(10, 10, 70), "PROD_C")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado.CaixasUtilizadas);
        Assert.Empty(resultado.ProdutosNaoEmbalados);

        var caixaUsada = resultado.CaixasUtilizadas.First();
        Assert.Equal("Caixa 1", caixaUsada.TipoCaixaUsada.Nome);
        Assert.Equal(3, caixaUsada.Produtos.Count);
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD_A");
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD_B");
        Assert.Contains(caixaUsada.Produtos, p => p.ProdutoId == "PROD_C");
    }

    [Fact]
    public void EmbalarPedido_ProdutosPreenchendoCaixasSequencialmente()
    {
        // Arrange
        var algoritmo = CriarAlgoritmo();
        var produtos = new List<Produto>
        {
            new Produto(new Dimensoes(28, 38, 78), "PROD_ENCHE_C1_1"),
            new Produto(new Dimensoes(28, 38, 78), "PROD_ENCHE_C1_2"),
            new Produto(new Dimensoes(70, 45, 35), "PROD_PARA_C2")
        };

        // Act
        ResultadoEmpacotamentoPedido resultado = algoritmo.EmbalarPedido(produtos);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.CaixasUtilizadas.Count);
        Assert.Empty(resultado.ProdutosNaoEmbalados);

        Assert.Contains(resultado.CaixasUtilizadas, c => c.TipoCaixaUsada.Nome == "Caixa 2" && c.Produtos.Any(p => p.ProdutoId == "PROD_PARA_C2"));
        Assert.Equal(2, resultado.CaixasUtilizadas.Count(c => c.TipoCaixaUsada.Nome == "Caixa 1"));
        
        var caixas1 = resultado.CaixasUtilizadas.Where(c => c.TipoCaixaUsada.Nome == "Caixa 1").ToList();
        Assert.Contains(caixas1[0].Produtos, p => p.ProdutoId == "PROD_ENCHE_C1_1");
        Assert.Contains(caixas1[1].Produtos, p => p.ProdutoId == "PROD_ENCHE_C1_2");
    }
}