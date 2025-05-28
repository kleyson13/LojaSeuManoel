using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Domain.Entities;

public class Produto
{
    public string? ProdutoId { get; set; } // Opcional, para identificar o produto na resposta
    public Dimensoes Dimensoes { get; }

    // Construtor usado ao criar a partir dos dados da requisição
    public Produto(Dimensoes dimensoes, string? produtoId = null)
    {
        Dimensoes = dimensoes ?? throw new ArgumentNullException(nameof(dimensoes));
        ProdutoId = produtoId;
    }
}