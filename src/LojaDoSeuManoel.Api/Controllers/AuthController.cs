using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LojaDoSeuManoel.Application.DTOs;

namespace LojaDoSeuManoel.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")] // Rota: /api/auth/login
    public IActionResult Login([FromBody] LoginRequestDto loginRequest)
    {
        if (loginRequest == null)
        {
            return BadRequest("Requisição de login inválida.");
        }

        _logger.LogInformation("Tentativa de login para o usuário: {Username}", loginRequest.Username);

        var testUsername = _configuration["TestUserCredentials:Username"];
        var testPassword = _configuration["TestUserCredentials:Password"];

        if (loginRequest.Username == testUsername && loginRequest.Password == testPassword)
        {
            var jwtKey = _configuration["Authentication:Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                _logger.LogError("Chave JWT não configurada corretamente ou muito curta.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro de configuração interna do servidor [JWT Key].");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Username!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var issuer = _configuration["Authentication:Jwt:Issuer"];
            var audience = _configuration["Authentication:Jwt:Audience"];
            var durationInMinutes = _configuration.GetValue<int>("Authentication:Jwt:DurationInMinutes", 60);
            var expiration = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);

            _logger.LogInformation("Token JWT gerado com sucesso para o usuário: {Username}", loginRequest.Username);

            return Ok(new LoginResponseDto
            {
                Token = stringToken,
                Expiration = expiration,
                Username = loginRequest.Username
            });
        }

        _logger.LogWarning("Falha na autenticação para o usuário: {Username}", loginRequest.Username);
        return Unauthorized("Usuário ou senha inválidos.");
    }
}