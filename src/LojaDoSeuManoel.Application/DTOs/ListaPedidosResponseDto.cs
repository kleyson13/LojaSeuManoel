using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class ListaPedidosResponseDto
{
    [JsonPropertyName("pedidos")]
    public List<PedidoProcessadoResponseDto> Pedidos { get; set; } = new();
}