# FCG.Users

Microsserviço responsável pelo cadastro, autenticação, autorização e gerenciamento de usuários da plataforma **FIAP Cloud Games (FCG)**.

A aplicação foi desenvolvida em **.NET 8**, seguindo princípios de **Clean Architecture**, **DDD**, **CQRS**, **SOLID** e arquitetura orientada a eventos.

---

## Responsabilidades

O `FCG.Users` é responsável por:

- cadastrar usuários;
- autenticar usuários;
- gerar tokens JWT;
- autorizar o acesso aos recursos da plataforma;
- consultar e gerenciar usuários;
- publicar o evento `UserCreatedEvent` após a criação de um usuário.

O evento publicado é consumido pelo microsserviço de notificações.

---

## Arquitetura

A solução está dividida nas seguintes camadas:

```text
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

- controllers;
- configuração da aplicação;
- Swagger/OpenAPI;
- autenticação e autorização;
- injeção de dependências;
- middlewares;
- exposição dos endpoints HTTP.

### FCG.Users.Application

Responsável por:

- comandos e consultas;
- handlers;
- DTOs;
- validações;
- casos de uso;
- orquestração da aplicação.

### FCG.Users.Domain

Responsável por:

- entidades;
- regras de negócio;
- interfaces de domínio;
- enums;
- comportamentos do domínio.

### FCG.Users.Infrastructure

Responsável por:

- persistência de dados;
- Entity Framework Core;
- SQL Server;
- repositórios;
- migrations;
- configuração do RabbitMQ;
- publicação de eventos de integração.

---

## Tecnologias

- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQL Server 2022
- RabbitMQ
- MassTransit
- JWT
- Swagger / OpenAPI
- Docker
- Docker Compose
- Kubernetes
- xUnit
- NuGet

---

## Dependência compartilhada

O microsserviço utiliza o pacote:

```text
FCG.BuildingBlocks
```

Referência utilizada nos projetos:

```xml
<PackageReference Include="FCG.BuildingBlocks" Version="1.0.1" />
```

---

## Banco de dados

Banco utilizado:

```text
SQL Server 2022
```

O banco é utilizado exclusivamente pelo microsserviço de usuários, mantendo a autonomia de dados esperada em uma arquitetura de microsserviços.

As migrations são executadas por meio do Entity Framework Core.

---

## Mensageria

Broker utilizado:

```text
RabbitMQ
```

Biblioteca utilizada:

```text
MassTransit
```

### Evento publicado

```text
UserCreatedEvent
```

Esse evento é publicado após o cadastro de um usuário e consumido pelo microsserviço de notificações.

### Eventos consumidos

Atualmente, o `FCG.Users` não consome eventos de outros microsserviços.

---

## Variáveis de ambiente

As configurações podem ser sobrescritas por variáveis de ambiente.

No .NET, dois sublinhados (`__`) representam a separação entre seções do `appsettings`.

Exemplo:

```text
ConnectionStrings__DefaultConnection
```

equivale a:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### Variáveis utilizadas

| Variável | Finalidade | Sensível |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação | Não |
| `ASPNETCORE_URLS` | Endereço e porta da API | Não |
| `ConnectionStrings__DefaultConnection` | Conexão com o SQL Server | Sim |
| `Jwt__SecretKey` | Chave de assinatura do JWT | Sim |
| `Jwt__Issuer` | Emissor do token | Não |
| `Jwt__Audience` | Público do token | Não |
| `Jwt__ExpirationMinutes` | Tempo de expiração do token | Não |
| `RabbitMq__Host` | Host do RabbitMQ | Não |
| `RabbitMq__Port` | Porta AMQP | Não |
| `RabbitMq__VirtualHost` | Virtual host do RabbitMQ | Não |
| `RabbitMq__Username` | Usuário do RabbitMQ | Sim |
| `RabbitMq__Password` | Senha do RabbitMQ | Sim |

No Kubernetes:

- configurações não sensíveis ficam no `ConfigMap`;
- connection string, chave JWT e credenciais ficam no `Secret`.

> Os valores presentes nos manifestos deste projeto são destinados apenas ao ambiente acadêmico e de demonstração. Em produção, recomenda-se utilizar um gerenciador de segredos, como Azure Key Vault, AWS Secrets Manager ou HashiCorp Vault.

---

## Executando localmente

### Pré-requisitos

- .NET SDK 8
- Docker Desktop
- SQL Server e RabbitMQ disponíveis
- Git

### Restaurar dependências

```powershell
dotnet restore --configfile .\NuGet.config
```

### Compilar

```powershell
dotnet build --no-restore
```

### Executar os testes

```powershell
dotnet test
```

### Executar a API

```powershell
dotnet run --project .\src\FCG.Users.Api\FCG.Users.Api.csproj
```

O endereço será exibido no terminal durante a inicialização.

Swagger:

```text
http://localhost:<porta>/swagger
```

---

## Docker

O `Dockerfile` utiliza **multi-stage build**.

A primeira etapa utiliza o SDK do .NET para restaurar, compilar e publicar a aplicação. A imagem final contém apenas o runtime e os arquivos publicados.

### Gerar a imagem

```powershell
docker build -t brnmatos/fcg-users-api:1.0.0 .
```

### Executar o container

Para execução isolada, a API precisa receber as configurações de banco e RabbitMQ por variáveis de ambiente e estar conectada à mesma rede da infraestrutura.

Exemplo simplificado:

```powershell
docker run -d `
  --name fcg-users-api `
  -p 5001:8080 `
  brnmatos/fcg-users-api:1.0.0
```

Swagger:

```text
http://localhost:5001/swagger
```

### Imagem publicada

```text
brnmatos/fcg-users-api:1.0.0
```

---

## Docker Compose

### Subir somente a infraestrutura

O arquivo `docker-compose.yml` inicia:

- SQL Server;
- RabbitMQ.

```powershell
docker compose up -d
```

### Subir a aplicação completa

O arquivo `docker-compose.full.yml` inicia:

- API de usuários;
- SQL Server;
- RabbitMQ.

```powershell
docker compose -f docker-compose.full.yml up -d --build
```

### Verificar os containers

```powershell
docker ps
```

### Acompanhar os logs

```powershell
docker compose -f docker-compose.full.yml logs -f
```

### Encerrar

```powershell
docker compose -f docker-compose.full.yml down
```

Para também remover volumes:

```powershell
docker compose -f docker-compose.full.yml down -v
```

---

## Kubernetes

Os manifestos estão na pasta:

```text
k8s
```

Todos os recursos são criados no namespace:

```text
fcg
```

### Recursos disponíveis

| Arquivo | Recurso |
|---|---|
| `namespace.yaml` | Namespace `fcg` |
| `configmap.yaml` | Configurações não sensíveis |
| `secret.yaml` | Credenciais e connection string |
| `deployment.yaml` | Deployment da API |
| `service.yaml` | Service interno da API |
| `sqlserver.yaml` | Deployment, Service e PVC do SQL Server |
| `rabbitmq.yaml` | Deployment e Service do RabbitMQ |

### Aplicar os manifestos

Crie primeiro o namespace:

```powershell
kubectl apply -f .\k8s\namespace.yaml
```

Depois aplique os demais arquivos:

```powershell
kubectl apply -f .\k8s\
```

### Verificar os recursos

```powershell
kubectl get all -n fcg
```

```powershell
kubectl get pods -n fcg
```

```powershell
kubectl get services -n fcg
```

```powershell
kubectl get configmaps -n fcg
```

```powershell
kubectl get secrets -n fcg
```

```powershell
kubectl get pvc -n fcg
```

### Acompanhar os logs da API

```powershell
kubectl logs -f deployment/fcg-users-api -n fcg
```

### Acessar a API

O serviço da API está configurado como **LoadBalancer**, permitindo acesso externo sem a necessidade de utilizar `kubectl port-forward`.

Para identificar o endereço disponível:

```powershell
kubectl get svc users-api -n fcg
```

Exemplo:

```text
NAME        TYPE           CLUSTER-IP      EXTERNAL-IP    PORT(S)
users-api   LoadBalancer   10.96.xxx.xxx   127.0.0.1      5001:xxxxx/TCP
```

Acesse o Swagger utilizando o endereço informado na coluna **EXTERNAL-IP**:

```text
http://<EXTERNAL-IP>:5001/swagger
```

No Docker Desktop, normalmente o endereço será:

```text
http://127.0.0.1:5001/swagger
```

### Acessar o RabbitMQ Management

O RabbitMQ está configurado como **ClusterIP**.

Para acessar o painel administrativo:

```powershell
kubectl port-forward service/rabbitmq 15672:15672 -n fcg
```

Depois acesse:

```text
http://127.0.0.1:15672
```

### Acessando o SQL Server pelo SQL Server Management Studio (SSMS)

O serviço do SQL Server está configurado como **ClusterIP**, portanto ele é acessível apenas pelos Pods dentro do cluster Kubernetes.

Para realizar conexões administrativas a partir da máquina local, utilize o comando abaixo para criar um túnel temporário entre o computador e o serviço do SQL Server:

```powershell
kubectl port-forward service/users-sqlserver 1436:1433 -n fcg
```

Enquanto o comando permanecer em execução, será possível conectar ao banco utilizando o SQL Server Management Studio (SSMS) com as seguintes configurações:

| Configuração | Valor |
|--------------|-------|
| Servidor | `127.0.0.1,1436` |
| Autenticação | SQL Server Authentication |
| Usuário | `sa` |
| Senha | A mesma configurada no `secret.yaml` |

> **Importante:** o `port-forward` permanece ativo apenas enquanto o terminal estiver aberto. Ao encerrá-lo, a conexão com o SQL Server será interrompida.

### Remover os recursos

```powershell
kubectl delete -f .\k8s\ -n fcg
```

Depois, se necessário:

```powershell
kubectl delete namespace fcg
```

---

## Comunicação dentro do Kubernetes

Os componentes utilizam os nomes dos Services do Kubernetes.

### SQL Server

```text
users-sqlserver:1433
```

### RabbitMQ

```text
rabbitmq:5672
```

### API de usuários

```text
users-api:80
```

Dentro de um container ou Pod, não deve ser utilizado `localhost` para acessar outro serviço.

---

## Testes

Os testes estão no projeto:

```text
tests/FCG.Users.Tests
```

Executar:

```powershell
dotnet test
```

Executar em modo Release:

```powershell
dotnet test -c Release
```

---

## Fluxo de cadastro

```text
Cliente
   |
   v
FCG.Users
   |
   | cadastra o usuário
   |
   v
SQL Server
   |
   | publica UserCreatedEvent
   |
   v
RabbitMQ
   |
   v
FCG.Notifications
   |
   | simula o envio
   |
   v
E-mail de boas-vindas
```

---

## CI/CD

O repositório está preparado para receber um pipeline com as seguintes etapas:

```text
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

A branch `main` utiliza regras de proteção e exige Pull Request para alterações.

---

## Segurança

- credenciais não devem ser mantidas no Dockerfile;
- senhas não devem ser gravadas diretamente no código;
- o `ConfigMap` não deve armazenar informações sensíveis;
- o `Secret` deve ser utilizado para credenciais;
- tokens e API Keys não devem ser versionados;
- em produção, recomenda-se um gerenciador externo de segredos.

---

## Autor

**Bruno Matos**

Pós-graduação em Arquitetura de Software — FIAP

Projeto desenvolvido para o Tech Challenge da FIAP, utilizando arquitetura de microsserviços, mensageria, Docker, Kubernetes e boas práticas de desenvolvimento em .NET.
