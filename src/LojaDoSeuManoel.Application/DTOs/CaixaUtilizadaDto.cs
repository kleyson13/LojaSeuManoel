namespace LojaDoSeuManoel.Application.DTOs;

public class CaixaUtilizadaDto
{
    public string NomeCaixa { get; set; } = string.Empty;
    public int AlturaCaixa { get; set; }
    public int LarguraCaixa { get; set; }
    public int ComprimentoCaixa { get; set; }
    public List<ProdutoDto> ProdutosNaCaixa { get; set; } = new List<ProdutoDto>();
}