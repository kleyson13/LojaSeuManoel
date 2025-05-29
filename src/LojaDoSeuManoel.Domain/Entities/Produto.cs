using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Domain.Entities;

public class Produto
{
    public string? ProdutoId { get; set; }
    public Dimensoes Dimensoes { get; }

    public Produto(Dimensoes dimensoes, string? produtoId = null)
    {
        Dimensoes = dimensoes ?? throw new ArgumentNullException(nameof(dimensoes));
        ProdutoId = produtoId;
    }
}