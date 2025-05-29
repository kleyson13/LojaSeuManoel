using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaDoSeuManoel.Infrastructure.Data.Entities;

[Table("TiposDeCaixa")]
public class TipoDeCaixaPersistencia
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public int AlturaCm { get; set; }
    public int LarguraCm { get; set; }
    public int ComprimentoCm { get; set; }
    public int VolumeCm3 { get; set; }
}