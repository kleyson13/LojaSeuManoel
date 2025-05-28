using LojaDoSeuManoel.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Application.Services;

public interface IPackingService
{
    // Recebe uma lista de pedidos (DTOs) e retorna uma lista de resultados de processamento (DTOs)
    Task<List<PedidoProcessadoDto>> ProcessarPedidosAsync(List<PedidoRequestDto> pedidos);
}