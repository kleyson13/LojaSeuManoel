using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LojaDoSeuManoel.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidosProcessados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoOriginalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataRecepcao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosProcessados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDeCaixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlturaCm = table.Column<int>(type: "int", nullable: false),
                    LarguraCm = table.Column<int>(type: "int", nullable: false),
                    ComprimentoCm = table.Column<int>(type: "int", nullable: false),
                    VolumeCm3 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDeCaixa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaixasUtilizadasNoPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoProcessadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeCaixa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlturaCaixaCm = table.Column<int>(type: "int", nullable: false),
                    LarguraCaixaCm = table.Column<int>(type: "int", nullable: false),
                    ComprimentoCaixaCm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixasUtilizadasNoPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaixasUtilizadasNoPedido_PedidosProcessados_PedidoProcessadoId",
                        column: x => x.PedidoProcessadoId,
                        principalTable: "PedidosProcessados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosNaCaixa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaixaUtilizadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoOriginalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlturaCm = table.Column<int>(type: "int", nullable: false),
                    LarguraCm = table.Column<int>(type: "int", nullable: false),
                    ComprimentoCm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosNaCaixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutosNaCaixa_CaixasUtilizadasNoPedido_CaixaUtilizadaId",
                        column: x => x.CaixaUtilizadaId,
                        principalTable: "CaixasUtilizadasNoPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TiposDeCaixa",
                columns: new[] { "Id", "AlturaCm", "ComprimentoCm", "LarguraCm", "Nome", "VolumeCm3" },
                values: new object[,]
                {
                    { 1, 30, 80, 40, "Caixa 1", 96000 },
                    { 2, 80, 40, 50, "Caixa 2", 160000 },
                    { 3, 50, 60, 80, "Caixa 3", 240000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaixasUtilizadasNoPedido_PedidoProcessadoId",
                table: "CaixasUtilizadasNoPedido",
                column: "PedidoProcessadoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosProcessados_PedidoOriginalId",
                table: "PedidosProcessados",
                column: "PedidoOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosNaCaixa_CaixaUtilizadaId",
                table: "ProdutosNaCaixa",
                column: "CaixaUtilizadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutosNaCaixa");

            migrationBuilder.DropTable(
                name: "TiposDeCaixa");

            migrationBuilder.DropTable(
                name: "CaixasUtilizadasNoPedido");

            migrationBuilder.DropTable(
                name: "PedidosProcessados");
        }
    }
}
