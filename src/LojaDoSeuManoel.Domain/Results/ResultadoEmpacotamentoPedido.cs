using LojaDoSeuManoel.Domain.Entities;
using System.Collections.Generic;

namespace LojaDoSeuManoel.Domain.Results;

public class ResultadoEmpacotamentoPedido
{
    public List<CaixaEmbalada> CaixasUtilizadas { get; }
    public List<Produto> ProdutosNaoEmbalados { get; }

    public ResultadoEmpacotamentoPedido()
    {
        CaixasUtilizadas = new List<CaixaEmbalada>();
        ProdutosNaoEmbalados = new List<Produto>();
    }
}