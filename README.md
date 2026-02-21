# 📨 Mensageria API — Envio de Arquivos por E-mail com RabbitMQ

API REST desenvolvida em **.NET 8 / C#** que permite o upload e persistência de arquivos binários em banco de dados **MySQL**, e o envio assíncrono desses arquivos por **e-mail** utilizando **RabbitMQ** como broker de mensageria e **MailHog** como servidor SMTP para testes.

---

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura do Projeto](#-arquitetura-do-projeto)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Pré-requisitos](#-pré-requisitos)
- [Como Rodar o Projeto](#-como-rodar-o-projeto)
- [Endpoints da API](#-endpoints-da-api)
- [Estrutura de Pastas](#-estrutura-de-pastas)
- [Variáveis e Configurações](#-variáveis-e-configurações)
- [Como Funciona o Fluxo](#-como-funciona-o-fluxo)

---

## 🔍 Visão Geral

O projeto **Mensageria API** resolve o seguinte cenário:

1. O usuário faz o **upload de um arquivo** (nome, tipo MIME e conteúdo em bytes) via API.
2. O arquivo é **salvo no banco de dados MySQL**.
3. Quando o arquivo é **consultado por ID**, o usuário fornece também um **e-mail de destino**.
4. A API **publica uma mensagem no RabbitMQ** contendo os dados do arquivo e o e-mail.
5. Um **Worker (background service)** consome a fila e **envia o arquivo como anexo por e-mail** via SMTP (MailHog).

Isso demonstra um padrão de **processamento assíncrono com mensageria**, desacoplando a lógica de envio de e-mail da requisição HTTP.

---

## 🏗️ Arquitetura do Projeto

O projeto segue o padrão de **Clean Architecture** em camadas:

```
┌─────────────────────────────────────────────────────┐
│                  MensageriaAPI                      │
│           (Camada de Apresentação)                  │
│         Controllers, Program.cs, Swagger            │
├─────────────────────────────────────────────────────┤
│               Mensageria.Domain                     │
│            (Camada de Domínio)                      │
│    Entities, DTOs, Events, Interfaces, Services     │
├──────────────────────┬──────────────────────────────┤
│  Mensageria.InfraData│   Mensageria.Infra.Message   │
│  (Banco de Dados)    │   (Mensageria / E-mail)      │
│  EF Core + MySQL     │   RabbitMQ + SMTP/MailHog    │
└──────────────────────┴──────────────────────────────┘
```

| Camada | Responsabilidade |
|---|---|
| **MensageriaAPI** | Recebe as requisições HTTP (controllers), configuração de DI e Swagger |
| **Mensageria.Domain** | Entidades, DTOs, eventos, interfaces e regras de negócio |
| **Mensageria.InfraData** | Acesso a dados com Entity Framework Core e MySQL (contexto, mapeamentos, repositórios) |
| **Mensageria.Infra.Message** | Publicação e consumo de mensagens no RabbitMQ, envio de e-mails via SMTP |

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| **.NET** | 8.0 | Framework principal |
| **C#** | 12 | Linguagem de programação |
| **Entity Framework Core** | 8.0.10 | ORM para acesso ao banco de dados |
| **Pomelo.EntityFrameworkCore.MySql** | 8.0.2 | Provider MySQL para EF Core |
| **MySQL** | 8.0 | Banco de dados relacional |
| **RabbitMQ** | 3 (Management) | Broker de mensageria |
| **RabbitMQ.Client** | 6.8.1 | Cliente .NET para RabbitMQ |
| **MailHog** | latest | Servidor SMTP fake para testes de e-mail |
| **Swashbuckle (Swagger)** | 6.6.2 | Documentação interativa da API |
| **Newtonsoft.Json** | 13.0.4 | Serialização/deserialização JSON |
| **Docker / Docker Compose** | — | Orquestração dos containers de infraestrutura |

---

## ✅ Pré-requisitos

Antes de rodar o projeto, certifique-se de ter instalado:

- [**.NET 8 SDK**](https://dotnet.microsoft.com/download/dotnet/8.0)
- [**Docker Desktop**](https://www.docker.com/products/docker-desktop/) (para rodar MySQL, RabbitMQ e MailHog)
- [**Git**](https://git-scm.com/) (para clonar o repositório)
- Um editor de código como **Visual Studio 2022** ou **VS Code** (com extensão C#)

---

## 🚀 Como Rodar o Projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/matheuspeluso/SendArchiveMensageAPI.git
cd SendArchiveMensageAPI
```

### 2. Subir os containers Docker

Na raiz do projeto (onde está o `docker-compose.yml`), execute:

```bash
docker-compose up -d
```

Isso irá inicializar:

| Serviço | Container | Porta(s) |
|---|---|---|
| **MySQL 8.0** | `mysql_db_mensageria` | `3306` |
| **RabbitMQ** | `rabbitmq_mensageria` | `5672` (AMQP) / `15672` (Painel Web) |
| **MailHog** | `mailhog_mensageria` | `1025` (SMTP) / `8025` (Interface Web) |

> 💡 **Dica:** Acesse o painel do RabbitMQ em `http://localhost:15672` (login: `DevMatheus` / senha: `FuscaAzul`) e a interface do MailHog em `http://localhost:8025` para visualizar os e-mails enviados.

### 3. Aplicar as migrations do banco de dados

```bash
cd Mensageria.InfraData
dotnet ef database update --startup-project ../MensageriaAPI
```

Ou, caso use o **Package Manager Console** no Visual Studio:

```
Update-Database -Project Mensageria.InfraData -StartupProject MensageriaAPI
```

### 4. Rodar a API

```bash
cd MensageriaAPI
dotnet run
```

Ou abra a solution `Mensageria.sln` no **Visual Studio** e pressione **F5**.

### 5. Acessar o Swagger

Com a API rodando, acesse a documentação interativa:

```
https://localhost:{porta}/swagger
```

> A porta será exibida no terminal ao iniciar o projeto. Geralmente `https://localhost:7xxx` ou `http://localhost:5xxx`.

---

## 📡 Endpoints da API

A API disponibiliza os seguintes endpoints no controller `api/Archive`:

### **POST** `/api/Archive` — Criar um arquivo

Salva um novo arquivo no banco de dados.

**Request Body (JSON):**
```json
{
  "name": "relatorio.pdf",
  "type": "application/pdf",
  "content": "<bytes do arquivo em base64>"
}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "relatorio.pdf",
  "type": "application/pdf",
  "content": "<bytes do arquivo em base64>"
}
```

---

### **GET** `/api/Archive/GetById/{id}?email={email}` — Consultar e enviar por e-mail

Busca o arquivo pelo ID e **envia uma cópia por e-mail** para o endereço informado (via fila RabbitMQ).

**Parâmetros:**
| Parâmetro | Tipo | Local | Descrição |
|---|---|---|---|
| `id` | `Guid` | URL path | ID do arquivo |
| `email` | `string` | Query string | E-mail de destino |

**Exemplo:**
```
GET /api/Archive/GetById/3fa85f64-5717-4562-b3fc-2c963f66afa6?email=usuario@email.com
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "relatorio.pdf",
  "type": "application/pdf",
  "content": "<bytes do arquivo em base64>"
}
```

> 📧 O e-mail com o arquivo em anexo será enviado de forma **assíncrona**. Visualize em `http://localhost:8025` (MailHog).

---

## 📁 Estrutura de Pastas

```
Mensageria/
├── docker-compose.yml              # Infraestrutura (MySQL, RabbitMQ, MailHog)
├── Mensageria.sln                  # Solution do projeto
│
├── MensageriaAPI/                  # 🌐 Camada de Apresentação
│   ├── Controllers/
│   │   └── ArchiveController.cs    # Endpoints REST
│   ├── Program.cs                  # Configuração e DI
│   ├── appsettings.json            # Configurações gerais
│   └── MensageriaAPI.csproj
│
├── Mensageria.Domain/              # 🧠 Camada de Domínio
│   ├── Entities/
│   │   └── Archive.cs              # Entidade principal (Id, Name, Type, Content)
│   ├── Dtos/
│   │   ├── ArchiveRequestDto.cs    # DTO de entrada
│   │   └── ArchiveResponseDto.cs   # DTO de saída
│   ├── Events/
│   │   └── SendArchiveEvents.cs    # Evento de mensageria
│   ├── Interfaces/
│   │   ├── Messages/
│   │   │   └── ISendArchiveMessage.cs
│   │   ├── Repositories/
│   │   │   └── IArchiveRepositories.cs
│   │   └── Services/
│   │       └── IArchiveServices.cs
│   └── Services/
│       └── ArchiveServices.cs      # Regras de negócio
│
├── Mensageria.InfraData/           # 💾 Camada de Dados
│   ├── Contexts/
│   │   └── DataContext.cs          # DbContext (EF Core + MySQL)
│   ├── Mappings/
│   │   └── ArchiveMap.cs           # Mapeamento da entidade → tabela
│   ├── Migrations/                 # Migrations do EF Core
│   └── Repositories/
│       └── ArchiveRepositories.cs  # Implementação do repositório
│
└── Mensageria.Infra.Message/       # 📨 Camada de Mensageria
    ├── Settings/
    │   └── RabbitMQSettings.cs     # Configurações do RabbitMQ
    ├── Publishers/
    │   └── SendArchivePublisher.cs # Publica mensagem na fila
    ├── Workers/
    │   └── SendArchiveWorker.cs    # Consome a fila (BackgroundService)
    ├── Components/
    │   └── SendEmail.cs            # Monta e envia o e-mail com anexo
    └── Helpers/
        └── SmtpMailHelper.cs       # Configuração do SMTP (MailHog)
```

---

## ⚙️ Variáveis e Configurações

### Banco de Dados (MySQL)

| Variável | Valor Padrão |
|---|---|
| Servidor | `localhost` |
| Porta | `3306` |
| Banco | `DBMensageria` |
| Usuário | `DevMatheus` |
| Senha | `FuscaAzul` |

> Configurado em `Mensageria.InfraData/Contexts/DataContext.cs`

### RabbitMQ

| Variável | Valor Padrão |
|---|---|
| Host | `localhost` |
| Porta AMQP | `5672` |
| Porta Painel | `15672` |
| Usuário | `DevMatheus` |
| Senha | `FuscaAzul` |
| Nome da Fila | `sendArchive` |

> Configurado em `Mensageria.Infra.Message/Settings/RabbitMQSettings.cs`

### SMTP (MailHog)

| Variável | Valor Padrão |
|---|---|
| Host | `localhost` |
| Porta | `1025` |
| Interface Web | `http://localhost:8025` |
| Remetente | `no-reply@mensageria.local` |

> Configurado em `Mensageria.Infra.Message/Helpers/SmtpMailHelper.cs`

---

## 🔄 Como Funciona o Fluxo

```
┌──────────┐     POST /api/Archive      ┌──────────────┐      Salva      ┌─────────┐
│  Cliente  │ ─────────────────────────► │  Controller  │ ──────────────► │  MySQL  │
└──────────┘                             └──────────────┘                 └─────────┘
     │
     │   GET /api/Archive/GetById/{id}?email=...
     │
     ▼
┌──────────────┐     Busca arquivo      ┌─────────┐
│  Controller  │ ─────────────────────► │  MySQL  │
└──────┬───────┘                        └─────────┘
       │
       │  Publica evento na fila
       ▼
┌──────────────┐                        ┌──────────────┐
│  Publisher   │ ─────────────────────► │  RabbitMQ    │
└──────────────┘     sendArchive        │  (Fila)      │
                                        └──────┬───────┘
                                               │
                                               │  Consome mensagem
                                               ▼
                                        ┌──────────────┐     Envia e-mail    ┌──────────┐
                                        │   Worker     │ ──────────────────► │  MailHog │
                                        │ (Background) │   com anexo (SMTP)  │  (SMTP)  │
                                        └──────────────┘                     └──────────┘
```

1. **POST** → O cliente envia o arquivo. O `ArchiveServices` cria a entidade e salva no MySQL via `ArchiveRepositories`.
2. **GET** → O cliente consulta o arquivo por ID e informa um e-mail. O `ArchiveServices` busca o arquivo e publica um `SendArchiveEvents` na fila `sendArchive` do RabbitMQ via `SendArchivePublisher`.
3. **Worker** → O `SendArchiveWorker` (BackgroundService) está escutando a fila. Ao consumir a mensagem, ele usa `SendEmail` para montar um e-mail HTML com o arquivo em anexo e envia via SMTP (MailHog).
4. Se o envio for bem-sucedido, a mensagem é confirmada (`BasicAck`). Se falhar, a mensagem volta para a fila (`BasicNack`) para reprocessamento.

---

## 📝 Licença

Este projeto é de uso educacional e demonstrativo.

---

> Desenvolvido com ❤️ utilizando .NET 8, RabbitMQ e Docker.

> Linkedin: https://www.linkedin.com/in/devmatheuspeluso/
> Email: matheuspeluso17@gmail.com
> Github: https://github.com/matheuspeluso
> WhatsApp: (21) 99385-7520