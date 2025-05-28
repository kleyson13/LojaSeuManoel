namespace LojaDoSeuManoel.Application.DTOs;

public class PedidoProcessadoDto
{
    public string? PedidoId { get; set; }
    public List<CaixaUtilizadaDto> Pacotes { get; set; } = new List<CaixaUtilizadaDto>();
}