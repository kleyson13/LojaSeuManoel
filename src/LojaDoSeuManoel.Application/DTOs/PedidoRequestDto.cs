namespace LojaDoSeuManoel.Application.DTOs;

public class PedidoRequestDto
{
    public string? PedidoId { get; set; }
    public List<ProdutoDto> Produtos { get; set; } = new List<ProdutoDto>();
}