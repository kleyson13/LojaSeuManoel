namespace LojaDoSeuManoel.Application.DTOs;

public class PedidoRequestDto
{
    public string? PedidoId { get; set; } // Opcional, para identificar o pedido na entrada/saída
    public List<ProdutoDto> Produtos { get; set; } = new List<ProdutoDto>();
}