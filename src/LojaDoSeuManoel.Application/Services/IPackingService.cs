using LojaDoSeuManoel.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Application.Services;

public interface IPackingService
{
    Task<List<PedidoProcessadoDto>> ProcessarPedidosAsync(List<PedidoRequestDto> pedidos);
}