using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class CaixaEmpacotadaResponseDto
{
    [JsonPropertyName("caixa_id")]
    public string? CaixaId { get; set; }

    [JsonPropertyName("produtos")]
    public List<string> Produtos { get; set; } = new();

    [JsonPropertyName("observacao")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Observacao { get; set; }
}