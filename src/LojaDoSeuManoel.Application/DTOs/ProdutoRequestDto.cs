using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class ProdutoRequestDto
{
    [JsonPropertyName("produto_id")]
    public string ProdutoId { get; set; } = string.Empty;

    [JsonPropertyName("dimensoes")]
    public DimensoesRequestDto Dimensoes { get; set; } = new();
}