# FCG.Users

Microsserviço .NET 8 responsável por cadastro, autenticação, autorização e gerenciamento de usuários da FIAP Cloud Games.

## Arquitetura da Fase 3

```text
Cliente
  -> Kong API Gateway
     -> FCG.Users.Api
        -> SQL Server
        -> HTTP POST -> Azure Function FCG.Notifications
        -> /metrics <- Prometheus <- Grafana
```

Na solução integrada, o Kong é o único ponto de entrada externo. Login e cadastro são públicos; as demais operações passam pela validação JWT do Gateway e continuam protegidas também pela API.

A configuração oficial de Kong, Prometheus, Grafana e Kubernetes compartilhado está no `FCG.Orchestration`.

## Responsabilidades

- cadastrar e gerenciar usuários;
- autenticar usuários e emitir tokens JWT;
- persistir os dados no banco SQL Server exclusivo do serviço;
- enviar os dados de usuário criado para Notifications via HTTP;
- expor métricas no formato Prometheus.

## Endpoints principais

| Método | Rota | Acesso no Kong |
|---|---|---|
| POST | `/api/v1/auth/login` | público |
| POST | `/api/v1/users/register` | público |
| POST | `/api/v1/users` | protegido |
| GET | `/api/v1/users` | protegido |
| GET | `/api/v1/users/{id}` | protegido |
| PUT | `/api/v1/users/{id}` | protegido |
| PATCH | `/api/v1/users/{id}/password` | protegido |
| PATCH | `/api/v1/users/{id}/profile` | protegido |
| PATCH | `/api/v1/users/{id}/activate` | protegido |
| PATCH | `/api/v1/users/{id}/inactivate` | protegido |

Swagger e o endpoint `/metrics` ficam disponíveis diretamente na API para desenvolvimento e coleta interna.

## Integração com Notifications

Depois de cadastrar o usuário, o serviço executa:

```text
POST {Notifications__BaseUrl}/api/notifications/user-created
x-functions-key: {Notifications__FunctionKey}  # quando configurada
Body: UserCreatedEvent em JSON
```

A chamada usa `HttpClient` criado por `IHttpClientFactory`. Uma resposta HTTP não bem-sucedida é tratada como falha da publicação. Users não publica essa notificação no RabbitMQ e não depende do broker.

Configurações:

| Variável | Finalidade | Sensível |
|---|---|---|
| `Notifications__BaseUrl` | URL base da Azure Function | não |
| `Notifications__FunctionKey` | Function Key enviada em `x-functions-key` | sim |

Para desenvolvimento com Functions Core Tools, a URL normalmente é `http://localhost:7071`. Em containers, use `http://host.docker.internal:7071` quando a Function estiver no host.

## JWT e Kong

```text
Issuer: FCG.Users.Api
Audience: FCG.CloudGames
Algorithm: HS256
```

O Kong localiza a credencial pelo claim `iss`. A chave `Jwt__SecretKey` deve ser idêntica na Users API, nas demais APIs e no Kong. O segredo não deve ser versionado.

No ambiente integrado, as rotas e o consumer JWT são declarados em `FCG.Orchestration/kong/kong.yml`; não há configuração manual obrigatória pelo Konga.

## Observabilidade

A API usa `prometheus-net.AspNetCore` e expõe:

```text
GET /metrics
```

O Prometheus do Orchestration coleta esse endpoint, e o dashboard provisionado no Grafana apresenta throughput, latência, status HTTP e erros.

## Configuração

Principais variáveis:

- `ConnectionStrings__DefaultConnection`
- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpirationMinutes`
- `Notifications__BaseUrl`
- `Notifications__FunctionKey`

Use `appsettings.Local.json` somente para valores locais não versionados. Secrets devem vir de variáveis de ambiente, Docker secrets, Kubernetes Secrets ou gerenciador externo.

## Execução local

```powershell
dotnet restore --configfile .\NuGet.config
dotnet build
dotnet test
dotnet run --project .\src\FCG.Users.Api
```

O perfil local expõe normalmente:

- Swagger: `http://localhost:5001/swagger`
- métricas: `http://localhost:5001/metrics`

## Docker

Build da API:

```powershell
docker build -f Dockerfile -t brnmatos/fcg-users-api:1.0.2 .
```

O `docker-compose.full.yml` mantém uma opção isolada para desenvolvimento do serviço. Para executar a arquitetura completa e reproduzível da Fase 3, use o `docker-compose.yml` do `FCG.Orchestration`, no qual a API é interna e acessada pelo Kong.

## Kubernetes

Os manifests em `k8s/` permitem desenvolvimento isolado da Users. Na implantação integrada, use os manifests e o Kustomize do `FCG.Orchestration`:

```powershell
kubectl apply -k .
```

Nesse ambiente:

- `users-api` é `ClusterIP` na porta `8080`;
- somente o proxy do Kong é `LoadBalancer`;
- SQL Server possui PVC;
- configurações não sensíveis ficam em ConfigMap;
- connection string, segredo JWT e Function Key ficam em Secret;
- readiness/liveness probes e requests/limits ficam no Deployment.

O `Notifications__BaseUrl` deve apontar para a Azure Function publicada, nunca para `fcg-notifications-functions` no cluster integrado.

## Fluxo de cadastro

```text
Cliente -> Kong -> Users API -> SQL Server
                            -> POST /api/notifications/user-created
                               -> Azure Function -> notificação de boas-vindas
```

## Segurança

- não versione JWT secret, Function Key ou connection string;
- mantenha login e registro como únicas rotas públicas de Users no Kong;
- preserve a validação JWT da própria API;
- exponha a API apenas internamente no Kubernetes integrado;
- armazene secrets em mecanismo apropriado ao ambiente.

## Relação com os requisitos da Fase 3

- API Gateway: atendido pelo Kong versionado no Orchestration;
- JWT: validado no Kong e na API;
- observabilidade: `/metrics` coletado por Prometheus e visualizado no Grafana;
- serverless: integração HTTP com a FCG.Notifications executada como Azure Function;
- RabbitMQ: não faz parte do fluxo de Users.
