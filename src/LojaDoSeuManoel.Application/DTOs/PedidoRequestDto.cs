﻿using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs;

public class PedidoRequestDto
{
    [JsonPropertyName("pedido_id")]
    public int PedidoId { get; set; }

    [JsonPropertyName("produtos")]
    public List<ProdutoRequestDto> Produtos { get; set; } = new();
}