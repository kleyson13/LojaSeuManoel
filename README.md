# Loja do Seu Manoel - API de Empacotamento 📦

[![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-blue)](https://docs.docker.com/compose/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
[![xUnit](https://img.shields.io/badge/Tests-xUnit-green)](https://xunit.net/)
[![Auth: JWT](https://img.shields.io/badge/Auth-JWT-brightgreen)](https://jwt.io/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Este projeto é uma solução para o desafio técnico "Loja do Seu Manoel", que consiste em desenvolver uma API para otimizar o processo de embalagem de pedidos. A API recebe uma lista de pedidos com produtos e suas dimensões e retorna a melhor forma de empacotá-los, minimizando o número de caixas utilizadas.

## Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura e Tecnologias](#arquitetura-e-tecnologias)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Como Rodar a Aplicação (Método Recomendado)](#como-rodar-a-aplicação-método-recomendado-com-docker)
- [Como Usar a API](#como-usar-a-api)
  - [Autenticação](#autenticação)
  - [Endpoint de Processamento](#endpoint-de-processamento)
  - [Exemplo de Requisição (JSON)](#exemplo-de-requisição-json)
  - [Exemplo de Resposta (JSON)](#exemplo-de-resposta-json)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Autor](#autor)

## Sobre o Projeto

Seu Manoel, dono de uma loja de jogos online, precisa automatizar a embalagem de seus pedidos. Esta API resolve o problema calculando, para cada pedido, quais das caixas disponíveis devem ser usadas e quais produtos devem ser colocados em cada uma. As caixas disponíveis são:

- **Caixa 1:** 30cm (Altura) x 40cm (Largura) x 80cm (Comprimento)
- **Caixa 2:** 80cm (Altura) x 50cm (Largura) x 40cm (Comprimento)
- **Caixa 3:** 50cm (Altura) x 80cm (Largura) x 60cm (Comprimento)

O núcleo do projeto aborda o problema de empacotamento (semelhante ao "Bin Packing Problem") através de um algoritmo heurístico. Este algoritmo considera as dimensões 3D dos produtos e suas rotações para determinar o encaixe nas caixas pré-definidas, buscando otimizar o espaço e minimizar o número de caixas usadas por pedido através de uma estratégia de preenchimento baseada em volume e encaixe individual.

## Arquitetura e Tecnologias

O projeto foi desenvolvido seguindo os princípios da **Clean Architecture**, separando as responsabilidades em camadas bem definidas para garantir alta coesão, baixo acoplamento, testabilidade e manutenibilidade.

**Principais Tecnologias Utilizadas:**

- **Backend:** .NET 9, ASP.NET Core Web API
- **Banco de Dados:** SQL Server 2022
- **ORM:** Entity Framework Core 9
- **Containerização:** Docker e Docker Compose
- **Testes:** xUnit
- **Documentação da API:** Swagger (OpenAPI)
- **Autenticação:** JWT (JSON Web Tokens)

## Funcionalidades

- [✔] Processamento em lote de múltiplos pedidos.
- [✔] Algoritmo de otimização para minimizar o número de caixas, considerando rotação de produtos.
- [✔] Persistência dos resultados do processamento em banco de dados.
- [✔] Documentação interativa da API com Swagger.
- [✔] Segurança de acesso via autenticação JWT Bearer.
- [✔] Cobertura de testes unitários para a lógica de negócio principal.
- [✔] Ambiente de desenvolvimento e produção totalmente containerizado.
- [✔] Tratamento de produtos que não cabem em nenhuma caixa disponível.

## Pré-requisitos

Para executar este projeto, você precisará ter instalado em sua máquina:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

O .NET 9 SDK só é necessário se você desejar rodar o projeto localmente, fora do ambiente Docker.

## Como Rodar a Aplicação (Método Recomendado com Docker)

A maneira mais simples e recomendada para executar a aplicação é utilizando o Docker Compose, que orquestra tanto a API quanto o banco de dados.

1.  **Clone o repositório:**

    ```bash
    git clone [https://github.com/kleyson13/LojaSeuManoel](https://github.com/kleyson13/LojaSeuManoel)
    cd LojaSeuManoel
    ```

2.  **Crie e configure o arquivo de ambiente (`.env`):**
    Na raiz do projeto, crie um arquivo chamado `.env`. Este arquivo guardará senhas e chaves secretas. Substitua os valores de exemplo pelos seus.

    ```env
    # .env
    SA_PASSWORD=SuaSenhaForteParaOBanco(!@#)

    # Configurações do JWT (substitua por valores seguros e adequados)
    JWT_KEY=SUA_CHAVE_SECRETA_SUPER_LONGA_E_COMPLEXA_PARA_JWT_AQUI_COM_PELO_MENOS_32_CARACTERES
    JWT_ISSUER=LojaDoSeuManoel.Api
    JWT_AUDIENCE=LojaDoSeuManoel.Usuarios
    ```

    **Importante:**

    - A `JWT_KEY` deve ser uma string longa, aleatória e secreta.
    - Adicione o arquivo `.env` ao seu `.gitignore` para não expor senhas e chaves no repositório.

3.  **Execute o Docker Compose:**
    No seu terminal, na raiz do projeto, execute o comando abaixo. Ele irá construir as imagens e iniciar os containers em segundo plano.

    ```bash
    docker-compose up --build -d
    ```

4.  **Acesse a API:**
    Após a conclusão do comando, a aplicação estará disponível:

    - **API / Swagger UI:** `http://localhost:8088` (ou a porta que você definiu no `docker-compose.yml`)
    - **Banco de Dados (SQL Server):** Acessível externamente em `localhost,14333` para ferramentas como SSMS ou Azure Data Studio.
      - **Login:** `sa`
      - **Senha:** A que você definiu para `SA_PASSWORD` no arquivo `.env`.

5.  **Parando a aplicação:**
    Para parar todos os containers, execute:
    ```bash
    docker-compose down
    ```

## Como Usar a API

### Autenticação

A API utiliza autenticação baseada em **JWT (JSON Web Tokens)**. Para acessar os endpoints protegidos, você primeiro precisa obter um token.

1.  **Obter um Token JWT:**
    Faça uma requisição `POST` para o endpoint `/api/auth/login` com as seguintes credenciais de teste no corpo (JSON):

    ```json
    {
      "username": "testeuser",
      "password": "Password123!"
    }
    ```

    _(Nota: Estas credenciais de teste estão configuradas no `appsettings.Development.json` da API apenas para fins de demonstração. A chave, emissor e audiência do JWT são lidos do arquivo `.env` quando rodando via Docker Compose, ou do `appsettings.Development.json` para execução local direta)._

    A resposta será um JSON contendo o token e sua data de expiração:

    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZXVzZXIiLCJqdGkiOiJjZTQxYjJlMy1mMzgzLTRkY2ItYWE5NS0xYjRkMGUzNDY4YjEiLCJleHAiOjE3MTcwODAwMDAsImlzcyI6IkxvSmFEb1NldU1hbm9lbC5BcGkiLCJhdWQiOiJMb2phRG9TZXVNYW5vZWwuVXN1YXJpb3MifQ.xxxxxxxxxxxxxxxxxxxxxxxxxxx",
      "expiration": "2025-05-30T18:00:00Z",
      "username": "testeuser"
    }
    ```

    _(O token e a data de expiração acima são apenas exemplos.)_

2.  **Usar o Token:**
    Para todas as requisições subsequentes aos endpoints protegidos (como `/api/pedidos`), inclua o token obtido no header HTTP `Authorization` usando o esquema `Bearer`:

    | Header          | Valor                                            |
    | --------------- | ------------------------------------------------ |
    | `Authorization` | `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...` |

    **No Swagger UI:**

    - Clique no botão "Authorize" no topo da página.
    - Na janela que abrir, no campo de definição de segurança "Bearer (apiKey)", cole o token JWT completo que você obteve, **prefixado com `Bearer `** (ex: `Bearer eyJhbGciOiJ...`).
    - Clique em "Authorize" e depois em "Close". As requisições subsequentes aos endpoints protegidos incluirão este token.

### Endpoint de Processamento

- **`POST /api/pedidos`** (Requer Autenticação JWT)
  - Recebe um objeto JSON contendo uma lista de pedidos no corpo da requisição.
  - Processa cada pedido e retorna um objeto JSON com os resultados do empacotamento.

### Exemplo de Requisição (JSON)

Este é o formato esperado para o corpo da sua requisição `POST /api/pedidos`:

```json
{
  "pedidos": [
    {
      "pedido_id": 1,
      "produtos": [
        {
          "produto_id": "PS5",
          "dimensoes": { "altura": 40, "largura": 10, "comprimento": 25 }
        },
        {
          "produto_id": "Volante",
          "dimensoes": { "altura": 40, "largura": 30, "comprimento": 30 }
        }
      ]
    },
    {
      "pedido_id": 2,
      "produtos": [
        {
          "produto_id": "Joystick",
          "dimensoes": { "altura": 15, "largura": 20, "comprimento": 10 }
        },
        {
          "produto_id": "Fifa 24",
          "dimensoes": { "altura": 10, "largura": 30, "comprimento": 10 }
        },
        {
          "produto_id": "Call of Duty",
          "dimensoes": { "altura": 30, "largura": 15, "comprimento": 10 }
        }
      ]
    },
    {
      "pedido_id": 5,
      "produtos": [
        {
          "produto_id": "Cadeira Gamer",
          "dimensoes": { "altura": 120, "largura": 60, "comprimento": 70 }
        }
      ]
    }
  ]
}
```

### Exemplo de Resposta (JSON)

```json
{
  "pedidos": [
    {
      "pedido_id": 1,
      "caixas": [
        {
          "caixa_id": "Caixa 2",
          "produtos": ["PS5", "Volante"]
        }
      ]
    },
    {
      "pedido_id": 2,
      "caixas": [
        {
          "caixa_id": "Caixa 1",
          "produtos": ["Joystick", "Fifa 24", "Call of Duty"]
        }
      ]
    },
    {
      "pedido_id": 5,
      "caixas": [
        {
          "caixa_id": null,
          "produtos": ["Cadeira Gamer"],
          "observacao": "Produto(s) não cabe(m) em nenhuma caixa disponível."
        }
      ]
    }
  ]
}
```

_(Nota: A atribuição específica de produtos às caixas "Caixa 1", "Caixa 2", "Caixa 3" na resposta acima é um exemplo ilustrativo. O algoritmo determinará a melhor combinação com base nas dimensões dos produtos e das caixas disponíveis.)_

## Testes

O projeto contém testes unitários para a lógica de negócio principal (o algoritmo de empacotamento). Para executá-los, rode o seguinte comando na raiz da solução:

```bash
dotnet test
```

## Estrutura do Projeto

A solução está organizada nas seguintes camadas/projetos:

```
/LojaSeuManoel
|
|-- src/
|   |-- LojaDoSeuManoel.Api/                # Camada de Apresentação (Controllers, DTOs, Program.cs, Dockerfile)
|   |-- LojaDoSeuManoel.Application/        # Camada de Aplicação (Lógica de orquestração, Serviços, DTOs)
|   |-- LojaDoSeuManoel.Domain/             # Camada de Domínio (Entidades, Regras de negócio, Algoritmo)
|   `-- LojaDoSeuManoel.Infrastructure/     # Camada de Infraestrutura (DbContext, Repositórios, Migrações)
|
|-- tests/
|   `-- LojaDoSeuManoel.Domain.Tests/       # Projeto de testes unitários para a camada de Domínio
|
|-- .env                                    # Arquivo de variáveis de ambiente (local)
|-- .gitignore                              # Arquivo para ignorar arquivos/pastas do Git
|-- LICENSE                                 # Liçensa MIT
|-- docker-compose.yml                      # Arquivo de orquestração dos containers
|-- LojaSeuManoel.sln                       # Arquivo da Solução .NET
`-- README.md                               # Esta documentação
```

## Autor

**Kleyson Mariano**

- Linkedin: [`linkedin.com/in/kleyson-mariano/`](https://www.linkedin.com/in/kleyson-mariano/)
- GitHub: [`github.com/kleyson13`](https://github.com/kleyson13)
