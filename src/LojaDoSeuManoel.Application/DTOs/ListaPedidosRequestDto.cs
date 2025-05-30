using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class ListaPedidosRequestDto
{
    [JsonPropertyName("pedidos")]
    public List<PedidoRequestDto> Pedidos { get; set; } = new();
}