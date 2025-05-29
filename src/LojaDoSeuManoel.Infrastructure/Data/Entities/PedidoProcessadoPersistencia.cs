using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaDoSeuManoel.Infrastructure.Data.Entities;

[Table("PedidosProcessados")]
public class PedidoProcessadoPersistencia
{
    [Key]
    public Guid Id { get; set; }

    public string? PedidoOriginalId { get; set; }
    public DateTime DataRecepcao { get; set; }

    public virtual ICollection<CaixaUtilizadaPersistencia> CaixasUtilizadas { get; set; } = new List<CaixaUtilizadaPersistencia>();
}