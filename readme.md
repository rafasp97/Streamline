# Streamline API

A Streamline API é uma aplicação desenvolvida para simular o fluxo completo de processamento de pedidos em um e-commerce moderno. Seu principal objetivo é fornecer uma plataforma robusta para gerenciar usuários, produtos e pedidos, integrando regras de negócio consistentes, mensageria assíncrona e processamento de dados confiável.

## 📌 Principais funcionalidades:

- **Gestão de Clientes (Customers):** permite a criação de usuários que realizam pedidos.
- **Gestão de Produtos (Products):** possibilita o cadastro produtos disponíveis para venda.
- **Processamento de Pedidos (Orders):** cria pedidos relacionando clientes e produtos, realizando o cálculo de valores, validação de estoque e registro de logs das operações.
- **Fluxo Assíncrono e Mensageria:** simula o processamento de pedidos em background, incluindo pagamento e atualização de status de entrega. Utiliza workers para processamentos agendados, retry de falhas e mensageria que simula notificações de envio e entrega ao cliente.

---

## 🛠️ Tecnologias

- **.NET 10**
- **SQL Server**
- **MongoDB**
- **RabbitMQ**
-- **MediatR**
- **Hangfire**
- **Docker Compose**
- **Entity Framework Core**

---

## ⚙️ Arquitetura utilizada
- **CQRS (Command Query Responsibility Segregation)**
- **DDD (Domain-Driven Design)**
- **Clean Architecture**

---

## 📁 Estrutura do Projeto

O projeto está organizado da seguinte forma:

```
Streamline
│
├── Api
│   ├── Routes               # Camada de apresentação da API (rotas)
│   ├── Mappings             # Mapeamentos DTOs → Commands/Queries
│   ├── DTOs                 # Data Transfer Objects usados na API
│   ├── Program.cs           # Configuração da API e integração com Mediator
│   ├── docker-compose.yml   # Orquestração da API e do PostgreSQL
│   └── .env                 # Variáveis de ambiente da API
│
├── Application
│   ├── Commands             # Comandos da aplicação
│   ├── CommandHandlers      # Handlers dos comandos (Mediator)
│   ├── Queries              # Queries da aplicação
│   ├── QueryHandlers        # Handlers das queries (Mediator)
│   ├── Results              # Classes de retorno de comandos/queries
│   └── Interfaces           # Interfaces da aplicação (repositórios, consumidores, serviços)
│
├── Domain
│   ├── Entities             # Entidades do domínio e regras de negócio
│   │   ├── Order
│   │   ├── Customer
│   │   ├── Product
│   │   ├── OrderProduct
│   │   └── Logs
│   └── Enums                # Enumerações do domínio
│
├── Infrastructure
│   ├── Repositories         # Implementação de repositórios
│   ├── Migrations           # Migrations do banco de dados
│   ├── Messaging            # Integração com mensageria
│   ├── Consumers            # Consumidores de filas/eventos
│   └── Workers              # Processamento assíncrono / background jobs

```

---

## Variáveis de Ambiente (Streamline.Api/.Env)

```env
# ========================
# SQL Server Configuration
# ========================

SQLSERVER_HOST=              # localhost
SQLSERVER_PORT=              # 1433
SQLSERVER_DATABASE=          # streamline
SQLSERVER_USER=              # sa
SQLSERVER_PASSWORD=          # MyRoot@123
SQLSERVER_TRUST_CERT=        # True

# ========================
# MongoDB Configuration
# ========================

MONGO_HOST=                  # localhost
MONGO_PORT=                  # 27017
MONGO_DATABASE=              # streamline
MONGO_USER=                  # admin
MONGO_PASS=                  # MyRoot@123
MONGO_URI=                   # mongodb://admin:MyRoot%40123@localhost:27017

# ========================
# RabbitMQ Configuration
# ========================

RABBITMQ_HOST=               # localhost
RABBITMQ_AMQP_PORT=          # 5672
RABBITMQ_MANAGEMENT_PORT=    # 15672
RABBITMQ_USER=               # admin
RABBITMQ_PASS=               # admin
RABBITMQ_URI=                # amqp://admin:admin@localhost:5672/
```

---

## ✅ Pré-requisitos

- Docker
- Docker Compose

---

## ▶️ Como Executar

1. **Clone o repositório:**

```bash
git clone https://github.com/rafasp97/Streamline.git
cd Streamline.Api
```

2. **Acesse a Camada de API**

```bash
cd Streamline.Api
```

3. **Execute o Docker Compose:**

```bash
docker compose up -d
```

4. **Execute para compilar e injetar as migrations**

```bash
dotnet ef database update --project ../Streamline.Infrastructure --startup-project .
```

---

## 🌐 Acessando a Aplicação

```bash
dotnet run
```

Após a execução, a API estará disponível em:

```
http://localhost/swagger/index.html
```

> A API está mapeada para a **porta 80**, portanto você não precisa informar a porta na URL.

## 📝 Observações

- Para cadastrar novos pedidos, é necessário a criação prévia de no mínimo um usuário e produtos.
