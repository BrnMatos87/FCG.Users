# FCG.Users

Microsserviço responsável pelo cadastro, autenticação, autorização e
gerenciamento de usuários da plataforma **FIAP Cloud Games (FCG)**.

A aplicação foi desenvolvida em **.NET 8**, seguindo princípios de
**Clean Architecture**, **DDD**, **CQRS**, **SOLID** e arquitetura
orientada a eventos.

Na Fase 3, o microsserviço evolui com **observabilidade via Prometheus**
e integração com o **Kong API Gateway**, mantendo os recursos
construídos nas fases anteriores.

------------------------------------------------------------------------

## Responsabilidades

O `FCG.Users` é responsável por:

-   cadastrar usuários;
-   autenticar usuários;
-   gerar tokens JWT;
-   autorizar o acesso aos recursos da plataforma;
-   consultar e gerenciar usuários;
-   publicar o evento `UserCreatedEvent` após a criação de um usuário;
-   expor métricas da aplicação para coleta pelo Prometheus.

O evento publicado é consumido pelo microsserviço de notificações.

O acesso externo à API, na arquitetura integrada da Fase 3, é realizado
por meio do **Kong API Gateway**.

------------------------------------------------------------------------

## Arquitetura

A solução está dividida nas seguintes camadas:

``` text
FCG.Users
│
├── src
│   ├── FCG.Users.Api
│   ├── FCG.Users.Application
│   ├── FCG.Users.Domain
│   └── FCG.Users.Infrastructure
│
├── tests
│   └── FCG.Users.Tests
│
├── k8s
│   ├── namespace.yaml
│   ├── configmap.yaml
│   ├── secret.yaml
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── sqlserver.yaml
│   └── rabbitmq.yaml
│
├── Dockerfile
├── docker-compose.yml
├── docker-compose.full.yml
├── NuGet.config
└── README.md
```

### FCG.Users.Api

Responsável por:

-   controllers;
-   configuração da aplicação;
-   Swagger/OpenAPI;
-   autenticação e autorização;
-   injeção de dependências;
-   middlewares;
-   exposição dos endpoints HTTP;
-   exposição do endpoint `/metrics` para o Prometheus.

### FCG.Users.Application

Responsável por:

-   comandos e consultas;
-   handlers;
-   DTOs;
-   validações;
-   casos de uso;
-   orquestração da aplicação.

### FCG.Users.Domain

Responsável por:

-   entidades;
-   regras de negócio;
-   interfaces de domínio;
-   enums;
-   comportamentos do domínio.

### FCG.Users.Infrastructure

Responsável por:

-   persistência de dados;
-   Entity Framework Core;
-   SQL Server;
-   repositórios;
-   migrations;
-   configuração do RabbitMQ;
-   publicação de eventos de integração.

------------------------------------------------------------------------

## Evolução --- Fase 3

Na Fase 3, o `FCG.Users` mantém a arquitetura construída anteriormente e
adiciona componentes de observabilidade e integração com API Gateway.

A arquitetura passa a considerar o seguinte fluxo:

``` text
Cliente
   |
   v
Kong API Gateway
   |
   | valida e roteia a requisição
   v
FCG.Users.Api
   |
   +------> SQL Server
   |
   +------> RabbitMQ
   |
   └------> /metrics
                |
                v
            Prometheus
                |
                v
             Grafana
```

O **Kong** atua como ponto de entrada da solução integrada, enquanto a
própria `FCG.Users.Api` continua mantendo sua autenticação e autorização
JWT.

A configuração centralizada do Gateway, incluindo Services, Routes e
políticas JWT, deve ser mantida no repositório **FCG.Orchestration**.

------------------------------------------------------------------------

## Tecnologias

-   .NET 8
-   ASP.NET Core
-   Entity Framework Core
-   SQL Server 2022
-   RabbitMQ
-   MassTransit
-   JWT
-   Swagger / OpenAPI
-   Prometheus
-   Kong API Gateway
-   Konga para administração local do Kong
-   Docker
-   Docker Compose
-   Kubernetes
-   xUnit
-   NuGet

------------------------------------------------------------------------

## Dependência compartilhada

O microsserviço utiliza o pacote:

``` text
FCG.BuildingBlocks
```

Referência utilizada nos projetos:

``` xml
<PackageReference Include="FCG.BuildingBlocks" Version="1.0.1" />
```

------------------------------------------------------------------------

## Banco de dados

Banco utilizado:

``` text
SQL Server 2022
```

O banco é utilizado exclusivamente pelo microsserviço de usuários,
mantendo a autonomia de dados esperada em uma arquitetura de
microsserviços.

As migrations são executadas por meio do Entity Framework Core.

------------------------------------------------------------------------

## Mensageria

Broker utilizado:

``` text
RabbitMQ
```

Biblioteca utilizada:

``` text
MassTransit
```

### Evento publicado

``` text
UserCreatedEvent
```

Esse evento é publicado após o cadastro de um usuário e consumido pelo
microsserviço de notificações.

### Eventos consumidos

Atualmente, o `FCG.Users` não consome eventos de outros microsserviços.

------------------------------------------------------------------------

## Autenticação e JWT

A `FCG.Users.Api` é responsável pela emissão dos tokens JWT utilizados
pela plataforma.

Configuração utilizada:

``` text
Issuer: FCG.Users.Api
Audience: FCG.CloudGames
ExpirationMinutes: 180
Algorithm: HS256
```

O token é assinado utilizando uma chave simétrica configurada por meio
de `Jwt__SecretKey`.

Na arquitetura da Fase 3 existem dois níveis de validação:

``` text
Cliente
   |
   | Bearer JWT
   v
Kong
   |
   | valida o JWT nas rotas protegidas
   v
FCG.Users.Api
   |
   | mantém sua própria autenticação/autorização
   v
Endpoint
```

Essa abordagem permite que requisições inválidas sejam rejeitadas pelo
Gateway antes de alcançar o microsserviço, sem remover a proteção
existente na aplicação.

------------------------------------------------------------------------

## API Gateway --- Kong

O **Kong API Gateway** é utilizado como ponto de entrada para as APIs da
plataforma.

Durante o desenvolvimento isolado da `FCG.Users`, Kong e Konga podem ser
executados pelo Docker Compose deste repositório para facilitar testes.

Na arquitetura integrada, a configuração oficial e centralizada do
Gateway pertence ao repositório:

``` text
FCG.Orchestration
```

### Service da Users

O Kong possui um Service que representa a API:

``` text
users-service
```

No ambiente Docker, o upstream utilizado é:

``` text
http://users-api:8080
```

### Rota pública de cadastro

O cadastro de usuários precisa permanecer acessível sem JWT.

Configuração:

``` text
Route: users-register-public-route
Path: /api/v1/users/register
Method: POST
Strip Path: false
JWT: não
```

Requisição pelo Gateway:

``` text
POST http://localhost:8000/api/v1/users/register
```

### Rotas protegidas

Os demais recursos de usuários são protegidos pelo plugin JWT do Kong.

Configuração principal:

``` text
Route: users-protected-route
Path: /api/v1/users
Strip Path: false
JWT: sim
```

Exemplo:

``` text
GET http://localhost:8000/api/v1/users
Authorization: Bearer <JWT>
```

### JWT Credential no Kong

A integração utiliza o `issuer` do token para localizar a credencial
correspondente:

``` text
Consumer: fcg-users
Key: FCG.Users.Api
Algorithm: HS256
key_claim_name: iss
```

A chave utilizada pelo Kong deve ser compatível com a chave utilizada
pela `FCG.Users.Api` para assinatura do token.

### Comportamento esperado

``` text
POST /api/v1/users/register
sem JWT
→ rota pública
→ requisição encaminhada para Users API

GET /api/v1/users
sem JWT
→ Kong
→ 401 Unauthorized

GET /api/v1/users
JWT inválido
→ Kong
→ 401 Unauthorized

GET /api/v1/users
JWT válido
→ Kong valida o token
→ encaminha para Users API
→ Users API mantém sua própria validação
```

> A configuração definitiva de Services, Routes, Consumers e políticas
> do Kong deve ser versionada no `FCG.Orchestration`. O Konga é
> utilizado como ferramenta de administração e apoio durante o
> desenvolvimento local.

------------------------------------------------------------------------

## Observabilidade --- Prometheus

A API utiliza `prometheus-net.AspNetCore` para disponibilizar métricas
no formato Prometheus.

Endpoint:

``` text
/metrics
```

Em execução local pelo Docker:

``` text
http://localhost:5001/metrics
```

No Kubernetes, o Deployment pode ser identificado pelo Prometheus por
meio das annotations:

``` yaml
prometheus.io/scrape: "true"
prometheus.io/path: "/metrics"
prometheus.io/port: "8080"
```

A infraestrutura centralizada de Prometheus e Grafana pertence ao
`FCG.Orchestration`.

------------------------------------------------------------------------

## Variáveis de ambiente

As configurações podem ser sobrescritas por variáveis de ambiente.

No .NET, dois sublinhados (`__`) representam a separação entre seções do
`appsettings`.

Exemplo:

``` text
ConnectionStrings__DefaultConnection
```

equivale a:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### Variáveis utilizadas

  ----------------------------------------------------------------------------------------
  Variável                                 Finalidade              Sensível
  ---------------------------------------- ----------------------- -----------------------
  `ASPNETCORE_ENVIRONMENT`                 Ambiente da aplicação   Não

  `ASPNETCORE_URLS`                        Endereço e porta da API Não

  `ConnectionStrings__DefaultConnection`   Conexão com o SQL       Sim
                                           Server                  

  `Jwt__SecretKey`                         Chave de assinatura do  Sim
                                           JWT                     

  `Jwt__Issuer`                            Emissor do token        Não

  `Jwt__Audience`                          Público do token        Não

  `Jwt__ExpirationMinutes`                 Tempo de expiração do   Não
                                           token (180 minutos)     

  `RabbitMq__Host`                         Host do RabbitMQ        Não

  `RabbitMq__Port`                         Porta AMQP              Não

  `RabbitMq__VirtualHost`                  Virtual host do         Não
                                           RabbitMQ                

  `RabbitMq__Username`                     Usuário do RabbitMQ     Sim

  `RabbitMq__Password`                     Senha do RabbitMQ       Sim
  ----------------------------------------------------------------------------------------

No Kubernetes:

-   configurações não sensíveis ficam no `ConfigMap`;
-   connection string, chave JWT e credenciais ficam no `Secret`.

> Os valores presentes nos manifestos deste projeto são destinados
> apenas ao ambiente acadêmico e de demonstração. Em produção,
> recomenda-se utilizar um gerenciador de segredos, como Azure Key
> Vault, AWS Secrets Manager ou HashiCorp Vault.

------------------------------------------------------------------------

## Executando localmente

### Pré-requisitos

-   .NET SDK 8
-   Docker Desktop
-   SQL Server e RabbitMQ disponíveis
-   Git

### Restaurar dependências

``` powershell
dotnet restore --configfile .\NuGet.config
```

### Compilar

``` powershell
dotnet build --no-restore
```

### Executar os testes

``` powershell
dotnet test
```

### Executar a API

``` powershell
dotnet run --project .\src\FCG.Users.Api\FCG.Users.Api.csproj
```

O endereço será exibido no terminal durante a inicialização.

Swagger:

``` text
http://localhost:<porta>/swagger
```

------------------------------------------------------------------------

## Docker

O `Dockerfile` utiliza **multi-stage build**.

A primeira etapa utiliza o SDK do .NET para restaurar, compilar e
publicar a aplicação. A imagem final contém apenas o runtime e os
arquivos publicados.

### Gerar a imagem

``` powershell
docker build -t brnmatos/fcg-users-api:1.0.1 .
```

### Executar o container

Para execução isolada, a API precisa receber as configurações de banco e
RabbitMQ por variáveis de ambiente e estar conectada à mesma rede da
infraestrutura.

Exemplo simplificado:

``` powershell
docker run -d `
  --name fcg-users-api `
  -p 5001:8080 `
  brnmatos/fcg-users-api:1.0.1
```

Swagger:

``` text
http://localhost:5001/swagger
```

### Imagem utilizada

``` text
brnmatos/fcg-users-api:1.0.1
```

------------------------------------------------------------------------

## Docker Compose

O projeto possui dois arquivos de Compose para facilitar diferentes
cenários de desenvolvimento.

### Subir somente a infraestrutura

O `docker-compose.yml` é utilizado para execução da infraestrutura local
sem iniciar a `FCG.Users.Api`.

Na configuração da Fase 3, o ambiente pode incluir:

-   SQL Server;
-   RabbitMQ;
-   Kong;
-   banco PostgreSQL utilizado pelo Kong;
-   Konga;
-   banco PostgreSQL utilizado pelo Konga.

``` powershell
docker compose up -d
```

Nesse cenário, a API pode ser executada separadamente pelo Visual Studio
ou `dotnet run`.

### Subir a aplicação completa

O `docker-compose.full.yml` inicia a API juntamente com sua
infraestrutura local:

-   FCG.Users.Api;
-   SQL Server;
-   RabbitMQ;
-   Kong;
-   PostgreSQL do Kong;
-   Konga;
-   PostgreSQL do Konga.

``` powershell
docker compose -f docker-compose.full.yml up -d --build
```

### Endereços locais

  Componente            Endereço
  --------------------- --------------------------------------------
  Users Swagger         `http://localhost:5001/swagger/index.html`
  Users Metrics         `http://localhost:5001/metrics`
  Kong Proxy            `http://localhost:8000`
  Kong Admin API        `http://localhost:8001`
  Konga                 `http://localhost:1337`
  RabbitMQ Management   `http://localhost:15672`

### Verificar os containers

``` powershell
docker ps
```

### Acompanhar os logs

``` powershell
docker compose -f docker-compose.full.yml logs -f
```

### Encerrar

``` powershell
docker compose -f docker-compose.full.yml down
```

Para também remover volumes:

``` powershell
docker compose -f docker-compose.full.yml down -v
```

> Ao remover os volumes do Kong/Konga, configurações criadas manualmente
> pelas interfaces administrativas também são removidas. A configuração
> definitiva do Gateway deve permanecer versionada no
> `FCG.Orchestration`.

------------------------------------------------------------------------

## Kubernetes

Os manifestos específicos da `FCG.Users` permanecem na pasta:

``` text
k8s
```

Todos os recursos são criados no namespace:

``` text
fcg
```

### Recursos disponíveis

  Arquivo             Recurso
  ------------------- -----------------------------------------
  `namespace.yaml`    Namespace `fcg`
  `configmap.yaml`    Configurações não sensíveis
  `secret.yaml`       Credenciais e connection string
  `deployment.yaml`   Deployment da API
  `service.yaml`      Service interno da API
  `sqlserver.yaml`    Deployment, Service e PVC do SQL Server
  `rabbitmq.yaml`     Deployment e Service do RabbitMQ

### Separação de responsabilidades

Os manifests desta pasta representam os recursos próprios da
`FCG.Users`.

Componentes compartilhados da solução, como:

-   Kong;
-   configuração centralizada das rotas do Gateway;
-   Prometheus;
-   Grafana;

devem ser centralizados no `FCG.Orchestration`.

### Service da API

Na arquitetura integrada da Fase 3, a `FCG.Users.Api` deve ser exposta
internamente no cluster por um `Service` do tipo `ClusterIP`.

O fluxo esperado é:

``` text
Cliente
   |
   v
Kong
   |
   v
users-api (ClusterIP)
   |
   v
FCG.Users.Api
```

Isso evita expor diretamente o microsserviço quando o Gateway é o ponto
de entrada da solução.

### Prometheus

O Deployment da API pode utilizar:

``` yaml
annotations:
  prometheus.io/scrape: "true"
  prometheus.io/path: "/metrics"
  prometheus.io/port: "8080"
```

### Aplicar os manifestos

Crie primeiro o namespace:

``` powershell
kubectl apply -f .\k8s\namespace.yaml
```

Depois aplique os demais arquivos:

``` powershell
kubectl apply -f .\k8s\
```

### Verificar os recursos

``` powershell
kubectl get all -n fcg
```

``` powershell
kubectl get pods -n fcg
```

``` powershell
kubectl get services -n fcg
```

``` powershell
kubectl get configmaps -n fcg
```

``` powershell
kubectl get secrets -n fcg
```

``` powershell
kubectl get pvc -n fcg
```

### Acompanhar os logs da API

``` powershell
kubectl logs -f deployment/fcg-users-api -n fcg
```

### Acessar a API para desenvolvimento

Como o `users-api` é interno (`ClusterIP`), quando for necessário
acessar diretamente a API para diagnóstico ou desenvolvimento local pode
ser utilizado `port-forward`:

``` powershell
kubectl port-forward service/users-api 5001:8080 -n fcg
```

Depois:

``` text
http://localhost:5001/swagger
```

Na arquitetura integrada, o acesso normal dos clientes deve ocorrer pelo
Kong.

### Acessar o RabbitMQ Management

O RabbitMQ está configurado como `ClusterIP`.

Para acessar o painel administrativo:

``` powershell
kubectl port-forward service/rabbitmq 15672:15672 -n fcg
```

Depois acesse:

``` text
http://127.0.0.1:15672
```

### Acessando o SQL Server pelo SQL Server Management Studio (SSMS)

O serviço do SQL Server está configurado como `ClusterIP`, portanto ele
é acessível apenas pelos Pods dentro do cluster Kubernetes.

Para realizar conexões administrativas a partir da máquina local:

``` powershell
kubectl port-forward service/users-sqlserver 1436:1433 -n fcg
```

Enquanto o comando permanecer em execução, será possível conectar ao
banco utilizando:

  Configuração   Valor
  -------------- --------------------------------------
  Servidor       `127.0.0.1,1436`
  Autenticação   SQL Server Authentication
  Usuário        `sa`
  Senha          A mesma configurada no `secret.yaml`

> O `port-forward` permanece ativo apenas enquanto o terminal estiver
> aberto.

### Remover os recursos

``` powershell
kubectl delete -f .\k8s\ -n fcg
```

Depois, se necessário:

``` powershell
kubectl delete namespace fcg
```

------------------------------------------------------------------------

## Comunicação dentro do Kubernetes

Os componentes utilizam os nomes dos Services do Kubernetes.

### SQL Server

``` text
users-sqlserver:1433
```

### RabbitMQ

``` text
rabbitmq:5672
```

### API de usuários

``` text
users-api:8080
```

Dentro de um container ou Pod, não deve ser utilizado `localhost` para
acessar outro serviço.

------------------------------------------------------------------------

## Testes

Os testes estão no projeto:

``` text
tests/FCG.Users.Tests
```

Executar:

``` powershell
dotnet test
```

Executar em modo Release:

``` powershell
dotnet test -c Release
```

------------------------------------------------------------------------

## Testando o Gateway

### Cadastro público

``` text
POST http://localhost:8000/api/v1/users/register
```

Não deve exigir JWT no Kong.

### Endpoint protegido sem token

``` text
GET http://localhost:8000/api/v1/users
```

Resultado esperado:

``` text
401 Unauthorized
```

### Endpoint protegido com token inválido

Envie:

``` text
Authorization: Bearer abc123
```

Resultado esperado:

``` text
401 Unauthorized
```

A rejeição deve ocorrer no Kong.

### Endpoint protegido com JWT válido

``` text
GET http://localhost:8000/api/v1/users
Authorization: Bearer <JWT>
```

Resultado esperado:

``` text
200 OK
```

O fluxo é:

``` text
Postman
   |
   | JWT
   v
Kong
   |
   | valida assinatura/expiração
   v
FCG.Users.Api
   |
   | mantém autenticação/autorização
   v
Resposta
```

------------------------------------------------------------------------

## Fluxo de cadastro

``` text
Cliente
   |
   v
Kong
   |
   | rota pública de cadastro
   v
FCG.Users
   |
   | cadastra o usuário
   v
SQL Server
   |
   | publica UserCreatedEvent
   v
RabbitMQ
   |
   v
FCG.Notifications
   |
   | simula o envio
   v
E-mail de boas-vindas
```

------------------------------------------------------------------------

## CI/CD

O repositório está preparado para receber um pipeline com as seguintes
etapas:

``` text
Restore
   |
   v
Build
   |
   v
Testes
   |
   v
Docker Build
   |
   v
Docker Push
   |
   v
Deploy Kubernetes
```

A branch `main` utiliza regras de proteção e exige Pull Request para
alterações.

------------------------------------------------------------------------

## Segurança

-   credenciais não devem ser mantidas no Dockerfile;
-   senhas não devem ser gravadas diretamente no código;
-   o `ConfigMap` não deve armazenar informações sensíveis;
-   o `Secret` deve ser utilizado para credenciais;
-   tokens e API Keys não devem ser versionados;
-   rotas públicas e protegidas são separadas no Kong;
-   o plugin JWT é aplicado apenas às rotas que exigem autenticação;
-   a `FCG.Users.Api` mantém sua própria validação JWT;
-   em produção, recomenda-se um gerenciador externo de segredos.

------------------------------------------------------------------------

## Integração com FCG.Orchestration

O repositório `FCG.Users` mantém os recursos necessários para
desenvolvimento e execução isolada do microsserviço.

A arquitetura completa da plataforma é centralizada pelo
`FCG.Orchestration`.

Responsabilidades previstas para o repositório de orquestração na Fase
3:

``` text
FCG.Orchestration
│
├── Kong API Gateway
│   ├── users-service
│   ├── catalog-service
│   ├── rotas públicas
│   ├── rotas protegidas
│   └── políticas JWT
│
├── Prometheus
├── Grafana
├── Redis
├── MongoDB
├── RabbitMQ
└── Kubernetes / Docker Compose
```

Dessa forma, o Kong utilizado localmente neste repositório serve para
desenvolvimento e validação da integração, enquanto a configuração
definitiva da arquitetura completa é versionada no `FCG.Orchestration`.

------------------------------------------------------------------------

## Autor

**Bruno Matos**

Pós-graduação em Arquitetura de Software --- FIAP

Projeto desenvolvido para o Tech Challenge da FIAP, utilizando
arquitetura de microsserviços, mensageria, Docker, Kubernetes, API
Gateway, observabilidade e boas práticas de desenvolvimento em .NET.
