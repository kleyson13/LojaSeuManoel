using LojaDoSeuManoel.Application.Services;
using LojaDoSeuManoel.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar servi�os ao container de Inje��o de Depend�ncia (DI).
// Isso diz ao ASP.NET Core: "Quando algu�m pedir um IPackingService, entregue uma inst�ncia de PackingService".
// AddScoped: Cria uma inst�ncia por requisi��o HTTP.
// AddTransient: Cria uma nova inst�ncia cada vez que � solicitado.
// AddSingleton: Cria uma �nica inst�ncia para toda a vida da aplica��o.

builder.Services.AddScoped<IPackingService, PackingService>();
// Se o AlgoritmoEmpacotamento n�o tiver estado e puder ser compartilhado (como no nosso caso atual),
// ele pode ser Singleton ou Scoped. Se ele tivesse estado que muda por requisi��o, seria Scoped ou Transient.
// Para simplificar, se PackingService j� o instancia, n�o precisamos registr�-lo separadamente aqui,
// a menos que PackingService o recebesse por DI tamb�m.
// Se PackingService recebesse AlgoritmoEmpacotamento por DI:
// builder.Services.AddScoped<AlgoritmoEmpacotamento>(); // Ou Transient, ou Singleton
// E o construtor de PackingService seria: public PackingService(AlgoritmoEmpacotamento algoritmo) { _algoritmoEmpacotamento = algoritmo; }


// Adiciona servi�os para controllers (necess�rio para que os controllers funcionem)
builder.Services.AddControllers();

// Adiciona servi�os para o Swagger/OpenAPI (documenta��o da API)
// Saiba mais em https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Necess�rio para o Swagger descobrir os endpoints
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Loja do Seu Manoel - API de Embalagens",
        Version = "v1",
        Description = "API para determinar a melhor forma de embalar produtos em caixas."
    });
});


var app = builder.Build(); // Cria a aplica��o

// 2. Configurar o pipeline de requisi��es HTTP (Middlewares).
// A ordem dos middlewares � importante.

// Se estiver em ambiente de desenvolvimento, habilita o Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Gera a especifica��o OpenAPI (JSON)
    app.UseSwaggerUI(options => // Serve a interface gr�fica do Swagger
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Loja do Seu Manoel API V1");
        options.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz (http://localhost:porta/)
    });
}

// app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS (comentado para simplificar testes locais sem certificado)

app.UseAuthorization(); // Aplica pol�ticas de autoriza��o (n�o estamos usando autentica��o/autoriza��o ainda)

app.MapControllers(); // Mapeia as rotas definidas nos controllers

app.Run(); // Inicia a aplica��o e come�a a ouvir requisi��es HTTP