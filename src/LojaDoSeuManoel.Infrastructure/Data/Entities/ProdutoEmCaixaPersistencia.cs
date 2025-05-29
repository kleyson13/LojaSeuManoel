using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaDoSeuManoel.Infrastructure.Data.Entities;

[Table("ProdutosNaCaixa")]
public class ProdutoEmCaixaPersistencia
{
    [Key]
    public Guid Id { get; set; }

    public Guid CaixaUtilizadaId { get; set; }
    [ForeignKey("CaixaUtilizadaId")]
    public virtual CaixaUtilizadaPersistencia? CaixaUtilizada { get; set; }

    public string? ProdutoOriginalId { get; set; }
    public int AlturaCm { get; set; }
    public int LarguraCm { get; set; }
    public int ComprimentoCm { get; set; }
}