namespace LojaDoSeuManoel.Application.DTOs;

public class LoginResponseDto
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
    public string? Username { get; set; }
}