namespace LojaDoSeuManoel.Application.DTOs;

public class ProdutoDto
{
    public string? ProdutoId { get; set; } // Opcional, para rastreabilidade
    public int Altura { get; set; }
    public int Largura { get; set; }
    public int Comprimento { get; set; }
}