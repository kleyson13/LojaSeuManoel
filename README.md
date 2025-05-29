## Rodando com Docker

Esta aplicação pode ser executada usando Docker e Docker Compose.

### Pré-requisitos

- Docker Desktop instalado e rodando.

### Passos para Executar

1.  Clone o repositório:

    ```bash
    git clone <URL_DO_SEU_REPOSITORIO>
    cd nome-do-projeto
    ```

2.  Crie um arquivo chamado `.env` na raiz do projeto (ao lado do `docker-compose.yml`) com o seguinte conteúdo, substituindo pela senha desejada para o usuário 'sa' do SQL Server:

    ```env
    SA_PASSWORD=yourStrong(!)Password123
    ```

    **Importante:** Adicione `.env` ao seu arquivo `.gitignore`!

3.  Execute o Docker Compose para construir as imagens e iniciar os containers:

    ```bash
    docker-compose up --build -d
    ```

    O `-d` executa os containers em segundo plano (detached mode). Se quiser ver os logs diretamente, omita o `-d`.

4.  A API estará disponível em `http://localhost:8088`.

    - O Swagger UI estará em `http://localhost:8088/swagger` (ou na raiz, dependendo da configuração).

5.  Para se conectar ao banco de dados SQL Server (que está rodando no Docker):
    - **Servidor:** `localhost,14333`
    - **Autenticação:** SQL Server Authentication
    - **Login:** `sa`
    - **Senha:** A senha definida no arquivo `.env`.
    - O nome do banco de dados criado será `LojaDoSeuManoelDB_Docker`.

### Parando os Containers

Para parar os containers em execução:

```bash
docker-compose down
```
