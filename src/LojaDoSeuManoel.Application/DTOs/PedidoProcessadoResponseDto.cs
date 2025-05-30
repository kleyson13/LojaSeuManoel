using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class PedidoProcessadoResponseDto
{
    [JsonPropertyName("pedido_id")]
    public int PedidoId { get; set; }

    [JsonPropertyName("caixas")]
    public List<CaixaEmpacotadaResponseDto> Caixas { get; set; } = new();
}