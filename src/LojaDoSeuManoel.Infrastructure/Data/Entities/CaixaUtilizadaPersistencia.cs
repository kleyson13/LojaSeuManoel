using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaDoSeuManoel.Infrastructure.Data.Entities;

[Table("CaixasUtilizadasNoPedido")]
public class CaixaUtilizadaPersistencia
{
    [Key]
    public Guid Id { get; set; }

    public Guid PedidoProcessadoId { get; set; }
    [ForeignKey("PedidoProcessadoId")]
    public virtual PedidoProcessadoPersistencia? PedidoProcessado { get; set; }

    public string NomeCaixa { get; set; } = string.Empty;
    public int AlturaCaixaCm { get; set; }
    public int LarguraCaixaCm { get; set; }
    public int ComprimentoCaixaCm { get; set; }

    public virtual ICollection<ProdutoEmCaixaPersistencia> ProdutosNaCaixa { get; set; } = new List<ProdutoEmCaixaPersistencia>();
}