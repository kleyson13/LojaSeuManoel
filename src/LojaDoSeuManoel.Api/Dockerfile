FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY src/LojaDoSeuManoel.Domain/LojaDoSeuManoel.Domain.csproj ./src/LojaDoSeuManoel.Domain/
COPY src/LojaDoSeuManoel.Application/LojaDoSeuManoel.Application.csproj ./src/LojaDoSeuManoel.Application/
COPY src/LojaDoSeuManoel.Infrastructure/LojaDoSeuManoel.Infrastructure.csproj ./src/LojaDoSeuManoel.Infrastructure/
COPY src/LojaDoSeuManoel.Api/LojaDoSeuManoel.Api.csproj ./src/LojaDoSeuManoel.Api/
COPY LojaSeuManoel.sln ./

RUN dotnet restore ./src/LojaDoSeuManoel.Api/LojaDoSeuManoel.Api.csproj

COPY src/. ./src/

WORKDIR /app/src/LojaDoSeuManoel.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build-env /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "LojaDoSeuManoel.Api.dll"]