using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class DimensoesRequestDto
{
    [JsonPropertyName("altura")]
    public int Altura { get; set; }

    [JsonPropertyName("largura")]
    public int Largura { get; set; }

    [JsonPropertyName("comprimento")]
    public int Comprimento { get; set; }
}