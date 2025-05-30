using LojaDoSeuManoel.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoSeuManoel.Domain.Services;

public class AlgoritmoEmpacotamento
{
    private readonly List<DefinicaoCaixa> _caixasDisponiveis;

    public AlgoritmoEmpacotamento(List<DefinicaoCaixa> caixasDisponiveis)
    {
        _caixasDisponiveis = caixasDisponiveis.OrderBy(c => c.Dimensoes.VolumeCm3).ToList();
        if (!_caixasDisponiveis.Any())
        {
            throw new ArgumentException("A lista de caixas disponíveis não pode ser vazia.", nameof(caixasDisponiveis));
        }
    }
    public List<CaixaEmbalada> EmbalarPedido(List<Produto> produtosDoPedido)
    {
        var caixasEmbaladasResultado = new List<CaixaEmbalada>();
        var produtosNaoEmbalados = produtosDoPedido
            .OrderByDescending(p => p.Dimensoes.VolumeCm3) 
            .ToList();

        while (produtosNaoEmbalados.Any())
        {
            CaixaEmbalada? melhorCaixaParaRodada = null;
            List<Produto>? produtosNaMelhorCaixaDaRodada = null;

            var produtoPrincipalDaRodada = produtosNaoEmbalados.First();

            foreach (var tipoCaixa in _caixasDisponiveis)
            {
                if (tipoCaixa.PodeConterProduto(produtoPrincipalDaRodada))
                {
                    var caixaAtual = new CaixaEmbalada(tipoCaixa);
                    var produtosAdicionadosNestaCaixa = new List<Produto>();

                    caixaAtual.AdicionarProduto(produtoPrincipalDaRodada);
                    produtosAdicionadosNestaCaixa.Add(produtoPrincipalDaRodada);

                    var produtosRestantesParaTentar = produtosNaoEmbalados
                        .Except(new List<Produto> { produtoPrincipalDaRodada })
                        .OrderByDescending(p => p.Dimensoes.VolumeCm3)
                        .ToList();

                    double volumeUsadoNaCaixaAtual = produtoPrincipalDaRodada.Dimensoes.VolumeCm3;

                    foreach (var pAdicional in produtosRestantesParaTentar)
                    {
                        if (tipoCaixa.PodeConterProduto(pAdicional) && 
                            (volumeUsadoNaCaixaAtual + pAdicional.Dimensoes.VolumeCm3) <= tipoCaixa.Dimensoes.VolumeCm3)
                        {
                            caixaAtual.AdicionarProduto(pAdicional);
                            produtosAdicionadosNestaCaixa.Add(pAdicional);
                            volumeUsadoNaCaixaAtual += pAdicional.Dimensoes.VolumeCm3;
                        }
                    }

                    if (produtosAdicionadosNestaCaixa.Any()) {
                        melhorCaixaParaRodada = new CaixaEmbalada(tipoCaixa);
                        foreach(var p in produtosAdicionadosNestaCaixa) {
                            melhorCaixaParaRodada.Produtos.Add(p);
                        }
                        produtosNaMelhorCaixaDaRodada = new List<Produto>(produtosAdicionadosNestaCaixa);
                        break;
                    }
                }
            }

            if (melhorCaixaParaRodada != null && produtosNaMelhorCaixaDaRodada != null && produtosNaMelhorCaixaDaRodada.Any())
            {
                caixasEmbaladasResultado.Add(melhorCaixaParaRodada);
                foreach (var produtoEmbalado in produtosNaMelhorCaixaDaRodada)
                {
                    produtosNaoEmbalados.Remove(produtoEmbalado);
                }
            }
            else if (produtosNaoEmbalados.Any())
            {
                var produtoProblematico = produtosNaoEmbalados.First();
                throw new InvalidOperationException($"Não foi possível embalar o produto {produtoProblematico.ProdutoId} (Dimensões: {produtoProblematico.Dimensoes}) pois é maior que todas as caixas disponíveis.");
            }
        }
        return caixasEmbaladasResultado;
    }
}