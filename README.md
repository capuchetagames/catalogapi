# Catalog API 🎮

API de catálogo de jogos desenvolvida em .NET 8, responsável pelo gerenciamento CRUD de jogos e iniciação do fluxo de compra. Parte de uma arquitetura de microsserviços para uma plataforma de jogos digitais.

## 📋 Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Endpoints da API](#endpoints-da-api)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Deployment](#deployment)
- [Estrutura do Projeto](#estrutura-do-projeto)

## 🎯 Sobre o Projeto

A Catalog API é um microsserviço que gerencia o catálogo de jogos da plataforma. Ela oferece funcionalidades completas de CRUD para jogos, autenticação distribuída via UserAPI, sistema de cache em memória para otimização de performance, e integração com RabbitMQ para processamento assíncrono de pedidos.

### Características Principais

- **CRUD Completo de Jogos**: Criação, leitura, atualização e exclusão de jogos
- **Biblioteca de Jogos do Usuário**: Gerenciamento da biblioteca pessoal de cada jogador
- **Sistema de Permissões**: Controle de acesso baseado em roles (Admin/User)
- **Autenticação Distribuída**: Validação de tokens JWT via UserAPI
- **Cache em Memória**: Otimização de performance com MemoryCache
- **Mensageria Assíncrona**: Integração com RabbitMQ para fluxo de compras
- **Health Checks**: Endpoints de monitoramento de saúde da aplicação

## ⚡ Funcionalidades

### Gestão de Jogos
- ✅ Listar todos os jogos (com cache)
- ✅ Buscar jogo por ID
- ✅ Criar novo jogo (somente Admin)
- ✅ Atualizar jogo existente (somente Admin)
- ✅ Deletar jogo (somente Admin)
- ✅ Iniciar ordem de compra

### Biblioteca do Jogador
- ✅ Visualizar biblioteca pessoal de jogos
- ✅ Histórico de compras

## 🛠 Tecnologias

### Backend
- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **SQL Server 2022** - Banco de dados

### Infraestrutura
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **Kubernetes** - Orquestração em produção
- **RabbitMQ** - Message Broker

### Bibliotecas e Ferramentas
- **Swagger/OpenAPI** - Documentação da API
- **MemoryCache** - Sistema de cache
- **Health Checks** - Monitoramento

## 🏗 Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação em camadas:

```
CatalogApi/
├── CatalogApi/          # Camada de apresentação (Controllers, Middlewares)
├── Core/                # Camada de domínio (Entities, DTOs, Interfaces)
└── Infrastructure/      # Camada de infraestrutura (Repositories, Migrations)
```

### Comunicação com Outros Serviços

- **UserAPI**: Validação de tokens JWT e autenticação distribuída
- **RabbitMQ**: Publicação de eventos de pedidos de compra
- **PaymentAPI** (consumidor): Processamento de pagamentos

## 📡 Endpoints da API

### Games Controller

#### `GET /api/Games`
Lista todos os jogos disponíveis.
- **Autenticação**: Requerida
- **Resposta**: `200 OK` - Lista de jogos

#### `GET /api/Games/{id}`
Busca um jogo específico por ID.
- **Autenticação**: Requerida
- **Parâmetros**: `id` (int)
- **Resposta**: `200 OK` - Objeto do jogo

#### `POST /api/Games`
Cria um novo jogo.
- **Autenticação**: Requerida (Admin)
- **Body**:
```json
{
  "name": "Nome do Jogo",
  "category": "Categoria",
  "price": 99.99,
  "active": true
}
```
- **Resposta**: `201 Created` - Jogo criado

#### `PUT /api/Games`
Atualiza um jogo existente.
- **Autenticação**: Requerida (Admin)
- **Body**:
```json
{
  "id": 1,
  "name": "Nome Atualizado",
  "category": "Nova Categoria",
  "price": 79.99,
  "active": true
}
```
- **Resposta**: `204 No Content`

#### `DELETE /api/Games/{id}`
Remove um jogo do catálogo.
- **Autenticação**: Requerida (Admin)
- **Parâmetros**: `id` (int)
- **Resposta**: `204 No Content`

#### `POST /order-game`
Cria uma ordem de compra para um jogo.
- **Autenticação**: Requerida
- **Body**:
```json
{
  "userId": 1,
  "gameId": 1
}
```
- **Resposta**: `201 Created` - Ordem criada

#### `GET /api/Games/health`
Verifica o status do serviço de catálogo.
- **Resposta**: `200 OK`

### Player Library Controller

#### `GET /api/PlayerLibrary/my-games`
Retorna a biblioteca de jogos do usuário autenticado.
- **Autenticação**: Requerida
- **Resposta**: `200 OK` - Lista de jogos do usuário

#### `GET /api/PlayerLibrary/health`
Verifica o status do serviço de biblioteca.
- **Resposta**: `200 OK`

### Health Check

#### `GET /health`
Endpoint de health check geral da aplicação.
- **Resposta**: `200 OK`

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/)
- [SQL Server 2022](https://www.microsoft.com/sql-server/sql-server-downloads) (ou usar via Docker)
- [RabbitMQ](https://www.rabbitmq.com/) (ou usar via Docker)

## 🚀 Instalação e Execução

### Opção 1: Docker Compose (Recomendado)

1. Clone o repositório:
```bash
git clone https://github.com/capuchetagames/catalogapi.git
cd catalogapi
```

2. Execute com Docker Compose:
```bash
docker-compose up -d
```

3. A API estará disponível em: `http://localhost:5245`

4. Acesse o Swagger: `http://localhost:5245/swagger`

### Opção 2: Execução Local

1. Clone o repositório:
```bash
git clone https://github.com/capuchetagames/catalogapi.git
cd catalogapi
```

2. Configure o banco de dados SQL Server (local ou via Docker):
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=rooot1234!!" -p 1436:1433 --name catalog_db -d mcr.microsoft.com/mssql/server:2022-latest
```

> ⚠️ **SEGURANÇA**: A senha acima é apenas para desenvolvimento local. Em produção, use senhas fortes e únicas!

3. Configure o RabbitMQ (local ou via Docker):
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin rabbitmq:3-management
```

> ⚠️ **SEGURANÇA**: As credenciais acima são apenas para desenvolvimento local. Em produção, use credenciais fortes e únicas!

4. Restaure as dependências:
```bash
dotnet restore
```

5. Execute as migrações do banco:
```bash
dotnet ef database update --project Infrastructure --startup-project CatalogApi
```

6. Execute a aplicação:
```bash
cd CatalogApi
dotnet run
```

7. A API estará disponível em: `http://localhost:5245`

## 🔐 Variáveis de Ambiente

### Arquivo `.env`
```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_HTTP_PORTS=5245
Jwt__Key=your-secret-jwt-key-here
```

> ⚠️ **IMPORTANTE**: Gere sua própria chave JWT segura. Nunca use chaves de exemplo em produção!

### Configuração no `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1436;Database=Db.Catalog;User Id=sa;Password=rooot1234!!;TrustServerCertificate=True;"
  },
  "RabbitMq": {
    "Host": "localhost",
    "User": "admin",
    "Password": "admin"
  },
  "Services": {
    "UsersApi": {
      "BaseUrl": "http://users-api:8080/"
    }
  }
}
```

> ⚠️ **NOTA DE SEGURANÇA**: As credenciais acima são valores de exemplo para desenvolvimento local. Em ambientes de produção:
> - Use senhas fortes e únicas para o banco de dados
> - Gere credenciais seguras para o RabbitMQ
> - Utilize secrets management (Azure Key Vault, AWS Secrets Manager, Kubernetes Secrets, etc.)
> - Nunca commite credenciais reais no controle de versão

### Variáveis Importantes

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | `Development` |
| `ASPNETCORE_HTTP_PORTS` | Porta HTTP | `5245` |
| `ConnectionStrings__DefaultConnection` | String de conexão do SQL Server | Ver appsettings.json |
| `RabbitMq__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMq__User` | Usuário do RabbitMQ | `admin` |
| `RabbitMq__Password` | Senha do RabbitMQ | `admin` |
| `Jwt__Key` | Chave secreta JWT | Gere uma chave segura única |
| `Services__UsersApi__BaseUrl` | URL base da UserAPI | `http://users-api:8080/` |

## 🐳 Deployment

### Docker

#### Build da Imagem
```bash
docker build -t catalogapi:latest .
```

#### Executar Container
```bash
docker run -d -p 5245:8080 --name catalog-api \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1436;Database=Db.Catalog;User Id=sa;Password=rooot1234!!;TrustServerCertificate=True;" \
  -e Jwt__Key="your-secret-jwt-key-here" \
  catalogapi:latest
```

> ⚠️ **SEGURANÇA**: 
> - Substitua `your-secret-jwt-key-here` por uma chave JWT segura
> - A senha do banco (`rooot1234!!`) é apenas para desenvolvimento local
> - Em produção, use credenciais fortes e gerenciadas por secrets management

### Kubernetes

O projeto inclui manifestos Kubernetes na pasta `/k8s`:

#### Deploy Completo
```bash
cd k8s
./k8s-start-all-deploy.sh
```

#### Deploy Individual

1. **Banco de Dados**:
```bash
./k8s-deploy-db.sh
```

2. **API**:
```bash
./k8s-deploy-api.sh
```

#### Recursos Kubernetes Disponíveis

- `catalog-deployment.yaml` - Deployment da API
- `catalog-service.yaml` - Service da API
- `catalog-configmap.yaml` - ConfigMap com configurações
- `catalog-secret.yaml` - Secrets (JWT, senhas)
- `sql-deployment.yaml` - Deployment do SQL Server
- `sql-service.yaml` - Service do SQL Server

#### Limpar Recursos
```bash
./k8s-delete-all.sh
```

## 📁 Estrutura do Projeto

```
catalogapi/
├── CatalogApi/                    # Camada de Apresentação
│   ├── Controllers/               # Controllers da API
│   │   ├── GamesController.cs
│   │   └── PlayerLibraryController.cs
│   ├── Middlewares/               # Middlewares customizados
│   ├── Service/                   # Serviços da aplicação
│   ├── Config/                    # Configurações
│   ├── Program.cs                 # Entry point
│   └── appsettings.json          # Configurações da aplicação
├── Core/                          # Camada de Domínio
│   ├── Entity/                    # Entidades de domínio
│   │   ├── Game.cs
│   │   └── PlayerLibraryGames.cs
│   ├── Dtos/                      # Data Transfer Objects
│   ├── Models/                    # Modelos
│   └── Repository/                # Interfaces de repositórios
├── Infrastructure/                # Camada de Infraestrutura
│   ├── Repository/                # Implementações de repositórios
│   ├── Migrations/                # Migrações do EF Core
│   └── Infrastructure.csproj
├── k8s/                           # Manifestos Kubernetes
│   ├── catalog-deployment.yaml
│   ├── catalog-service.yaml
│   ├── sql-deployment.yaml
│   └── scripts de deploy
├── docker-compose.yaml            # Orquestração Docker
├── Dockerfile                     # Imagem Docker
├── .dockerignore
├── .gitignore
└── README.md
```

## 🤝 Microsserviços Relacionados

Este projeto faz parte de uma arquitetura de microsserviços:

- **UserAPI** - Gerenciamento de usuários e autenticação
- **PaymentAPI** - Processamento de pagamentos
- **CatalogAPI** (este projeto) - Catálogo de jogos

## 📝 Licença

Este projeto é parte do ecossistema Capucheta Games.

## 👥 Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📞 Suporte

Para suporte e dúvidas, entre em contato com a equipe de desenvolvimento.

---

Desenvolvido com ❤️ pela equipe Capucheta Games
