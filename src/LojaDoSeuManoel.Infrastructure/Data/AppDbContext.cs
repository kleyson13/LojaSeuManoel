using LojaDoSeuManoel.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LojaDoSeuManoel.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TipoDeCaixaPersistencia> TiposDeCaixa { get; set; }
    public DbSet<PedidoProcessadoPersistencia> PedidosProcessados { get; set; }
    public DbSet<CaixaUtilizadaPersistencia> CaixasUtilizadas { get; set; }
    public DbSet<ProdutoEmCaixaPersistencia> ProdutosNaCaixa { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PedidoProcessadoPersistencia>()
            .HasIndex(p => p.PedidoOriginalId);

        modelBuilder.Entity<TipoDeCaixaPersistencia>().HasData(
            new TipoDeCaixaPersistencia { Id = 1, Nome = "Caixa 1", AlturaCm = 30, LarguraCm = 40, ComprimentoCm = 80, VolumeCm3 = 30*40*80 },
            new TipoDeCaixaPersistencia { Id = 2, Nome = "Caixa 2", AlturaCm = 80, LarguraCm = 50, ComprimentoCm = 40, VolumeCm3 = 80*50*40 },
            new TipoDeCaixaPersistencia { Id = 3, Nome = "Caixa 3", AlturaCm = 50, LarguraCm = 80, ComprimentoCm = 60, VolumeCm3 = 50*80*60 }
        );
    }
}