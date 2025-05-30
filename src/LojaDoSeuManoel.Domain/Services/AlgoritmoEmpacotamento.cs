using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Results;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoSeuManoel.Domain.Services;

public class AlgoritmoEmpacotamento
{
    private readonly List<DefinicaoCaixa> _caixasDisponiveis;

    public AlgoritmoEmpacotamento(List<DefinicaoCaixa> caixasDisponiveis)
    {
        _caixasDisponiveis = caixasDisponiveis
            .OrderBy(c => c.Dimensoes.VolumeCm3)
            .ToList();

        if (!_caixasDisponiveis.Any())
        {
            throw new ArgumentException("A lista de caixas disponíveis não pode ser vazia.", nameof(caixasDisponiveis));
        }
    }

    public ResultadoEmpacotamentoPedido EmbalarPedido(List<Produto> produtosDoPedido)
    {
        var resultado = new ResultadoEmpacotamentoPedido();
        
        var produtosAindaPorEmbalar = produtosDoPedido
            .OrderByDescending(p => p.Dimensoes.VolumeCm3)
            .ToList();

        while (produtosAindaPorEmbalar.Any())
        {
            var produtoPrincipalDaRodada = produtosAindaPorEmbalar.First();

            CaixaEmbalada? caixaEscolhidaParaEstaRodada = null;
            List<Produto> produtosEmbaladosNestaRodada = new List<Produto>();
            bool produtoPrincipalPodeSerEmbalado = false;

            foreach (var tipoCaixa in _caixasDisponiveis)
            {
                if (tipoCaixa.PodeConterProduto(produtoPrincipalDaRodada))
                {
                    produtoPrincipalPodeSerEmbalado = true;
                    var caixaAtual = new CaixaEmbalada(tipoCaixa);
                    
                    caixaAtual.AdicionarProduto(produtoPrincipalDaRodada);
                    produtosEmbaladosNestaRodada.Add(produtoPrincipalDaRodada);
                    
                    double volumeUsadoNaCaixaAtual = produtoPrincipalDaRodada.Dimensoes.VolumeCm3;

                    var outrosProdutosParaTentar = produtosAindaPorEmbalar
                        .Except(new List<Produto> { produtoPrincipalDaRodada })
                        .OrderByDescending(p => p.Dimensoes.VolumeCm3) 
                        .ToList();

                    foreach (var produtoAdicional in outrosProdutosParaTentar)
                    {
                        if (tipoCaixa.PodeConterProduto(produtoAdicional) &&
                            (volumeUsadoNaCaixaAtual + produtoAdicional.Dimensoes.VolumeCm3) <= tipoCaixa.Dimensoes.VolumeCm3)
                        {
                            caixaAtual.AdicionarProduto(produtoAdicional);
                            produtosEmbaladosNestaRodada.Add(produtoAdicional);
                            volumeUsadoNaCaixaAtual += produtoAdicional.Dimensoes.VolumeCm3;
                        }
                    }
                    
                    caixaEscolhidaParaEstaRodada = caixaAtual;
                    break;
                }
            }

            if (produtoPrincipalPodeSerEmbalado && caixaEscolhidaParaEstaRodada != null)
            {
                resultado.CaixasUtilizadas.Add(caixaEscolhidaParaEstaRodada);
                foreach (var produtoEmbalado in produtosEmbaladosNestaRodada)
                {
                    produtosAindaPorEmbalar.Remove(produtoEmbalado);
                }
            }
            else
            {
                resultado.ProdutosNaoEmbalados.Add(produtoPrincipalDaRodada);
                produtosAindaPorEmbalar.Remove(produtoPrincipalDaRodada);
                Console.WriteLine($"AVISO ALGORITMO: Produto {produtoPrincipalDaRodada.ProdutoId} (Dimensões: {produtoPrincipalDaRodada.Dimensoes}) não coube em nenhuma caixa e foi adicionado à lista de não embalados.");
            }
        }

        return resultado;
    }
}