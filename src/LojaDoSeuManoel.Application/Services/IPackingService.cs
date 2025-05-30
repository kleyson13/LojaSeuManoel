using LojaDoSeuManoel.Application.DTOs;

namespace LojaDoSeuManoel.Application.Services;

public interface IPackingService
{
    Task<List<PedidoProcessadoResponseDto>> ProcessarListaDePedidosAsync(List<PedidoRequestDto> pedidos);
}