using System.Collections.Generic;
using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Domain.Entities;

public class CaixaEmbalada
{
    public DefinicaoCaixa TipoCaixaUsada { get; }
    public List<Produto> Produtos { get; } // Produtos colocados nesta caixa

    public CaixaEmbalada(DefinicaoCaixa tipoCaixaUsada)
    {
        TipoCaixaUsada = tipoCaixaUsada;
        Produtos = new List<Produto>();
    }

    // Método simplificado para tentar adicionar um produto.
    // Um algoritmo real aqui seria muito mais complexo (3D packing).
    // Para o teste, vamos assumir que se o produto individualmente cabe (considerando rotação)
    // e o volume total não estoura (muito simplista), ele pode ser adicionado.
    // O algoritmo principal fará a escolha de qual caixa usar.
    public bool AdicionarProduto(Produto produto)
    {
        // O PackingService é que vai decidir se o produto vai para ESTA caixa ou outra.
        // Este método só adiciona à lista se a decisão já foi tomada.
        // A verificação se 'cabe' com outros itens já na caixa é o desafio NP-Hard.
        // Para este exercício, o algoritmo de empacotamento principal vai decidir
        // quais produtos vão para uma instância desta CaixaEmbalada.
        // Aqui, apenas adicionamos.
        if (TipoCaixaUsada.PodeConterProduto(produto)) // Garante que individualmente caberia
        {
            Produtos.Add(produto);
            return true;
        }
        return false; // Não deveria acontecer se o algoritmo escolheu bem
    }
}