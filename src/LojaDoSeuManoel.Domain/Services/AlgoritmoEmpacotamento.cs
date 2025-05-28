using LojaDoSeuManoel.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoSeuManoel.Domain.Services;

public class AlgoritmoEmpacotamento
{
    private readonly List<DefinicaoCaixa> _caixasDisponiveis;

    public AlgoritmoEmpacotamento()
    {
        // Inicializa as caixas disponíveis (poderia vir de um repositório/configuração)
        _caixasDisponiveis = new List<DefinicaoCaixa>
        {
            new DefinicaoCaixa("Caixa 1", new ValueObjects.Dimensoes(30, 40, 80)), // A, L, C
            new DefinicaoCaixa("Caixa 2", new ValueObjects.Dimensoes(80, 50, 40)),
            new DefinicaoCaixa("Caixa 3", new ValueObjects.Dimensoes(50, 80, 60)),
        }.OrderBy(c => c.Dimensoes.VolumeCm3).ToList(); // Ordenar por volume pode ser uma estratégia
    }

    public List<CaixaEmbalada> EmbalarPedido(List<Produto> produtosDoPedido)
    {
        var caixasEmbaladasResultado = new List<CaixaEmbalada>();
        var produtosNaoEmbalados = produtosDoPedido
            .OrderByDescending(p => p.Dimensoes.VolumeCm3) // Estratégia: maiores primeiro
            .ToList();

        while (produtosNaoEmbalados.Any())
        {
            CaixaEmbalada? melhorCaixaParaRodada = null;
            List<Produto>? produtosNaMelhorCaixaDaRodada = null;

            // Tenta encontrar a melhor caixa para os produtos restantes
            foreach (var tipoCaixa in _caixasDisponiveis)
            {
                // Tenta encaixar o máximo de produtosNaoEmbalados nesta tipoCaixa
                var produtosQueCabemNestaCaixa = new List<Produto>();
                var copiaProdutosNaoEmbalados = new List<Produto>(produtosNaoEmbalados); // Trabalha com uma cópia para esta simulação

                // Simplificação: Não estamos gerenciando o espaço 3D restante na caixa.
                // Apenas vemos quais produtos cabem individualmente e tentamos colocar o máximo possível.
                // Uma abordagem é tentar colocar o primeiro (maior) produto que ainda não foi embalado.
                // E depois, tentar adicionar outros que caibam JUNTOS com ele (o que é a parte difícil).

                // Heurística simples: Pegar o primeiro produto que cabe, e depois ver quais outros cabem.
                // Para o teste: vamos focar em pegar o primeiro produto (o maior dos restantes)
                // e colocá-lo na menor caixa possível. Depois, tentar adicionar mais a essa caixa.

                var produtoAtualParaEmpacotar = copiaProdutosNaoEmbalados.FirstOrDefault();
                if (produtoAtualParaEmpacotar == null) break; // Não há mais produtos

                if (tipoCaixa.PodeConterProduto(produtoAtualParaEmpacotar))
                {
                    // Se este tipo de caixa pode conter o produto atual...
                    // Vamos simular colocar este e outros produtos nela.
                    var caixaSimulada = new CaixaEmbalada(tipoCaixa);
                    caixaSimulada.AdicionarProduto(produtoAtualParaEmpacotar); // Adiciona o primeiro
                    produtosQueCabemNestaCaixa.Add(produtoAtualParaEmpacotar);

                    // Tenta adicionar mais produtos (dos restantes) a esta caixa simulada
                    // Esta é uma simplificação ENORME. Um bom algoritmo tentaria otimizar o espaço 3D.
                    // Para o desafio, "minimizar caixas" pode ser alcançado por:
                    // 1. Escolher a menor caixa que caiba o item atual.
                    // 2. Tentar colocar mais itens nessa caixa antes de fechar.
                    foreach (var outroProduto in copiaProdutosNaoEmbalados.Skip(1)) // Pula o que já foi adicionado
                    {
                        // Verificação muito simplista: se individualmente cabe, e o volume total não explode
                        // Uma melhoria seria verificar se o *volume somado* cabe, e se individualmente cabe.
                        // Mas o ideal é um algoritmo de "encaixe" 3D.
                        if (tipoCaixa.PodeConterProduto(outroProduto) &&
                            (caixaSimulada.Produtos.Sum(p => p.Dimensoes.VolumeCm3) + outroProduto.Dimensoes.VolumeCm3 <= tipoCaixa.Dimensoes.VolumeCm3 * 0.95) // Fator de "folga"
                           )
                        {
                            // A verificação acima é perigosa, pois não garante encaixe 3D.
                            // Para o teste, vamos manter mais simples:
                            // Se um produto foi escolhido para a caixa, tentamos preencher essa caixa
                            // com outros produtos que *individualmente* caberiam nela.
                            // Esta é a estratégia da "Primeira Caixa que Cabe, depois Tenta Encher".
                            produtosQueCabemNestaCaixa.Add(outroProduto);
                            // Não adicionamos à caixaSimulada aqui ainda, só coletamos candidatos.
                        }
                    }

                    // Agora, temos uma 'tipoCaixa' e uma lista de 'produtosQueCabemNestaCaixa'
                    // Se é a primeira caixa que encontramos ou se esta caixa embala mais produtos (ou usa uma caixa menor para a mesma qtd)
                    if (melhorCaixaParaRodada == null || produtosQueCabemNestaCaixa.Count > produtosNaMelhorCaixaDaRodada!.Count)
                    {
                        // Se estamos tentando minimizar caixas, e esta caixa tem produtos.
                        // A estratégia aqui é: achou uma caixa que cabe o primeiro produto da lista (o maior não embalado)?
                        // Use-a e tente colocar mais produtos nela.

                        // Heurística: Encher a primeira caixa encontrada que caiba o maior produto atual.
                        var caixaReal = new CaixaEmbalada(tipoCaixa);
                        var produtosRealmenteAdicionadosNestaCaixa = new List<Produto>();

                        // Adiciona o produto principal que direcionou a escolha desta caixa
                        caixaReal.AdicionarProduto(produtoAtualParaEmpacotar);
                        produtosRealmenteAdicionadosNestaCaixa.Add(produtoAtualParaEmpacotar);

                        // Tenta adicionar mais produtos dos restantes
                        var tempProdutosNaoEmbalados = produtosNaoEmbalados
                            .Where(p => p != produtoAtualParaEmpacotar) // Exclui o que já vai
                            .OrderByDescending(p => p.Dimensoes.VolumeCm3) // Reordena os restantes
                            .ToList();

                        foreach (var pAdicional in tempProdutosNaoEmbalados)
                        {
                            // Simplificação: se individualmente cabe, adiciona. Não estamos verificando
                            // o espaço restante de forma inteligente.
                            if (tipoCaixa.PodeConterProduto(pAdicional))
                            {
                                // E se o volume acumulado não estourar (de forma bem bruta)
                                if ((caixaReal.Produtos.Sum(pr => pr.Dimensoes.VolumeCm3) + pAdicional.Dimensoes.VolumeCm3) <= tipoCaixa.Dimensoes.VolumeCm3)
                                {
                                    caixaReal.AdicionarProduto(pAdicional);
                                    produtosRealmenteAdicionadosNestaCaixa.Add(pAdicional);
                                }
                            }
                        }

                        melhorCaixaParaRodada = caixaReal;
                        produtosNaMelhorCaixaDaRodada = produtosRealmenteAdicionadosNestaCaixa;
                        break; // Achamos uma caixa e tentamos enchê-la. Vamos para a próxima rodada com os produtos que sobraram.
                               // Esta estratégia é "First Fit" para o produto maior, depois tenta preencher.
                    }
                }
            } // Fim do foreach tipoCaixa

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
                // Se chegou aqui, nenhum produto restante pôde ser embalado.
                // Pode ser um produto muito grande ou um erro na lógica.
                // Para o teste, você pode lançar uma exceção ou retornar um status especial.
                // Ex: throw new InvalidOperationException($"Não foi possível embalar os produtos restantes. Produto grande demais? {produtosNaoEmbalados.First().ProdutoId}");
                Console.WriteLine($"AVISO: Não foi possível embalar {produtosNaoEmbalados.Count} produtos restantes. O primeiro é: {produtosNaoEmbalados.FirstOrDefault()?.ProdutoId}");
                break; // Interrompe para evitar loop infinito.
            }
        } // Fim do while produtosNaoEmbalados

        return caixasEmbaladasResultado;
    }
}