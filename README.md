# Catalog API 🎮

API de catálogo de jogos desenvolvida em .NET 8, responsável pelo CRUD de jogos, busca textual no catálogo, publicação de pedidos de compra e consumo de eventos de pagamento.

## Sobre o projeto

A Catalog API faz parte de uma arquitetura de microsserviços da plataforma de jogos e atualmente inclui:

- CRUD de jogos
- Busca de jogos com Elasticsearch
- Cache distribuído com Redis
- Persistência com Entity Framework Core + PostgreSQL
- Publicação e consumo de eventos com RabbitMQ
- Logging em DynamoDB
- Health checks da aplicação

## Funcionalidades

### Gestão de jogos
-  Listar jogos
-  Buscar jogo por ID
-  Criar jogo (somente Admin)
-  Atualizar jogo (somente Admin)
-  Deletar jogo (somente Admin)
-  Criar ordem de compra (`order.events`)
-  Buscar jogos com texto e filtro de categoria
-  Reindexar catálogo no Elasticsearch

### Biblioteca do jogador
-  Endpoint de biblioteca (`my-games`)
-  Endpoint de health da biblioteca

## Stack atual

### Backend
- .NET 8 / ASP.NET Core
- Entity Framework Core
- Npgsql (PostgreSQL)

### Infra e integrações
- PostgreSQL 16
- Redis (cache distribuído)
- Elasticsearch 8
- RabbitMQ
- DynamoDB (logs)
- Docker / Docker Compose
- Kubernetes (manifestos na pasta `k8s/`)

### Bibliotecas
- Swashbuckle (Swagger)
- FluentValidation
- AWSSDK.DynamoDBv2
- RabbitMQ.Client
- Elastic.Clients.Elasticsearch
- Serilog + New Relic enrichers

## Arquitetura

```
catalogapi/
├── CatalogApi/        # Camada de apresentação (Controllers, Middlewares, Services)
├── Core/              # Camada de domínio (Entities, DTOs, interfaces)
└── Infrastructure/    # Infraestrutura (EF Core, repositórios, Elasticsearch)
```

## Endpoints principais

### Games
- `GET /api/Games`
- `GET /api/Games/{id}`
- `POST /api/Games`
- `PUT /api/Games`
- `DELETE /api/Games/{id}`
- `POST /order-game`
- `GET /api/Games/search?q={termo}&category={categoria?}`
- `POST /api/Games/reindex`
- `GET /api/Games/health`

### PlayerLibrary
- `GET /api/PlayerLibrary/my-games`
- `GET /api/PlayerLibrary/health`

### Health global
- `GET /health`

## Autenticação

O projeto possui serviço e middleware para validação de JWT via UserAPI (`TokenValidationService` e `JwtValidationMiddleware`), porém o middleware customizado está desabilitado por padrão em `Program.cs`.

## Mensageria

### Publicação de pedido
- Exchange: `order.events`
- Routing key: `order.ordered`
- Endpoint: `POST /order-game`

### Consumo de pagamento
- Exchange: `payments.events`
- Queue: `payments.process`
- Routing key: `payment.*`
- Consumer: `PaymentProcessConsumer`

## Pré-requisitos

- .NET SDK 8.0
- Docker e Docker Compose
- PostgreSQL
- Redis
- Elasticsearch
- RabbitMQ
- DynamoDB (ou DynamoDB Local, para desenvolvimento)

## Execução local

### 1) Configurar variáveis

```bash
cp .env.example .env
```

Preencha os valores do `.env`.

### 2) Subir infra local (PostgreSQL, Redis, Elasticsearch)

```bash
docker compose -f docker-compose.local.yaml up -d
```

### 3) Subir serviços auxiliares (RabbitMQ e DynamoDB Local)

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin \
  rabbitmq:3-management


docker run -d --name dynamodb-local \
  -p 8000:8000 \
  amazon/dynamodb-local
```

### 4) Rodar API com Docker

```bash
docker compose -f docker-compose.api.yaml up -d --build
```

API: `http://localhost:5245`  
Swagger (Development): `http://localhost:5245/swagger`

## Execução com dotnet

```bash
dotnet restore CatalogApi.sln
dotnet build CatalogApi.sln -c Release
cd CatalogApi
dotnet run
```

## Variáveis de ambiente

Arquivo base: `.env.example`

Principais variáveis:

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_HTTP_PORTS`
- `DB_CONNECTION_STRING`
- `PG_USER`
- `PG_PASSWORD`
- `Jwt__Key`
- `REDIS_CONNECTION`
- `DynamoDb__LogTableName`
- `DynamoDb__UseLocal`
- `DynamoDb__LocalUrl`
- `DynamoDb__Region`
- `DynamoDb__ProfileName`
- `ElasticSettings__LocalUrl`
- `ElasticSettings__UseCloud`
- `ElasticSettings__ApiKey`
- `ElasticSettings__CloudId`

## CI/CD

Pipeline em `.github/workflows/ci-cd.yml` com etapas de:

1. Build e teste (`dotnet restore`, `dotnet build`, `dotnet test`)
2. Build/push de imagem Docker
3. Security scan (Trivy)
4. Deploy em EKS (rolling update)

### Limpeza de segredos em commits antigos (histórico Git)

Se algum segredo já foi commitado no passado, apenas editar o arquivo atual não é suficiente. Use este fluxo:

1. **Inventário**
   - Identifique todos os valores e padrões vazados no histórico.
2. **Rotação imediata**
   - Rotacione/revogue todos os segredos expostos (DB, JWT, AWS, Elastic etc.).
3. **Reescrita de histórico**
   - Use `git-filter-repo` para remover/substituir os valores em todos os commits, branches e tags.
4. **Validação**
   - Garanta que nenhum segredo apareça mais no histórico reescrito.
5. **Publicação**
   - Faça push forçado de branches/tags reescritas para o remoto.
6. **Sincronização do time**
   - Oriente o time a re-clonar ou resetar os clones locais para o novo histórico.
7. **Retenção externa**
   - Revise PRs, forks, logs e artefatos de CI que possam ter retido os segredos.
8. **Prevenção contínua**
   - Secret scanning ativo, push protection e revisão obrigatória de segredos em PRs.

Comandos de referência:

```bash
pip install git-filter-repo
git filter-repo --replace-text replacements.txt --force
git push origin --force --all
git push origin --force --tags
```

Exemplo de `replacements.txt`:

```txt
literal:valor_real_do_segredo==>PLACEHOLDER_SEGURO
regex:Jwt__Key:\s*"[^"]+"==>Jwt__Key: "JWT_KEY_PLACEHOLDER"
regex:POSTGRES_PASSWORD:\s*"[^"]+"==>POSTGRES_PASSWORD: "POSTGRES_PASSWORD_PLACEHOLDER"
```

## Estrutura de arquivos relevante

```
catalogapi/
├── CatalogApi/
│   ├── Controllers/
│   ├── Middlewares/
│   ├── Service/
│   ├── Program.cs
│   └── appsettings*.json
├── Core/
├── Infrastructure/
├── docker-compose.api.yaml
├── docker-compose.local.yaml
├── Dockerfile
└── k8s/
```

## Microsserviços relacionados

- UserAPI (autenticação)
- PaymentAPI (pagamentos)
- CatalogAPI (este repositório)
