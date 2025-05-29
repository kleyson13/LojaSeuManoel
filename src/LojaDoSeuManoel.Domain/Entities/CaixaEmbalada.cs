using System.Collections.Generic;
using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Domain.Entities;

public class CaixaEmbalada
{
    public DefinicaoCaixa TipoCaixaUsada { get; }
    public List<Produto> Produtos { get; }

    public CaixaEmbalada(DefinicaoCaixa tipoCaixaUsada)
    {
        TipoCaixaUsada = tipoCaixaUsada;
        Produtos = new List<Produto>();
    }

    public bool AdicionarProduto(Produto produto)
    {
        if (TipoCaixaUsada.PodeConterProduto(produto))
        {
            Produtos.Add(produto);
            return true;
        }
        return false;
    }
}