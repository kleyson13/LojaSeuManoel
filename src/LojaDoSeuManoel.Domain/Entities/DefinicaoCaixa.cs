using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Domain.Entities;

public class DefinicaoCaixa
{
    public string Nome { get; }
    public Dimensoes Dimensoes { get; }

    public DefinicaoCaixa(string nome, Dimensoes dimensoes)
    {
        Nome = nome;
        Dimensoes = dimensoes;
    }

    // Verifica se um produto (com suas possíveis rotações) cabe nesta definição de caixa
    public bool PodeConterProduto(Produto produto)
    {
        foreach (var rotacaoProduto in produto.Dimensoes.ObterRotacoes())
        {
            if (rotacaoProduto.CabeEm(this.Dimensoes))
            {
                return true;
            }
        }
        return false;
    }
}