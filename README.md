<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-14-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Architecture | DDD-Clean%20%2B%20CQRS-blueviolet?style=for-the-badge" />
</p>

<h1 align="center">🛒 ECommerce Platform</h1>

<p align="center">
  <b>A production-ready, enterprise-grade e-commerce backend built with .NET 10</b><br/>
  <sub>Modular Monolith • Clean Architecture • CQRS • DDD</sub>
</p>

<p align="center">
  <a href="#-features">Features</a> •
  <a href="#%EF%B8%8F-architecture">Architecture</a> •
  <a href="#-project-structure">Structure</a> •
  <a href="#-getting-started">Getting Started</a> •
  <a href="#-api-reference">API</a> •
  <a href="#-security">Security</a> •
  <a href="#-roadmap">Roadmap</a>
</p>

---

## ✨ Features

<table>
<tr>
<td width="50%">

### 🛍️ Customer Experience
- **Guest checkout** — shop without registration
- **Membership system** — optional account creation
- **Smart basket** — persistent cart for both guests & members
- **Product catalog** — browse by categories
- **Secure payments** — tokenized credit card processing
- **Order tracking** — real-time order status

</td>
<td width="50%">

### 🔧 Admin Panel (White-Label)
- **Product management** — full CRUD operations
- **Stock control** — real-time inventory updates
- **Customer management** — view all registered users
- **Order management** — view & manage all orders
- **Sales overview** — track revenue & transactions
- **Role-based access** — Admin-gated endpoints

</td>
</tr>
</table>

---

## 🏗️ Architecture

This project follows **Modular Monolith** architecture with strict **Clean Architecture** layer separation, **CQRS** pattern via MediatR, and **Domain-Driven Design** principles.

```
┌──────────────────────────────────────────────────────────┐
│                      API Layer                           │
│            (Controllers • Middlewares • DI)               │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────────┐  ┌──────────────────────────────┐  │
│  │  Infrastructure  │  │        Persistence           │  │
│  │  ───────────────  │  │  ────────────────────────── │  │
│  │  • JWT Service   │  │  • EF Core DbContext        │  │
│  │  • Payment       │  │  • Fluent API Configs       │  │
│  │  • Serilog       │  │  • Repositories             │  │
│  │                  │  │  • Soft-Delete Interceptor   │  │
│  └────────┬─────────┘  └─────────────┬────────────────┘  │
│           │                          │                   │
│           ▼                          ▼                   │
│  ┌───────────────────────────────────────────────────┐   │
│  │              Application Layer                     │   │
│  │  ─────────────────────────────────────────────── │   │
│  │  • Commands & Queries (CQRS via MediatR)          │   │
│  │  • Validation Pipeline (FluentValidation)         │   │
│  │  • Logging Pipeline                               │   │
│  │  • Interface Abstractions (IApplicationDbContext)  │   │
│  └──────────────────────┬────────────────────────────┘   │
│                         │                                │
│                         ▼                                │
│  ┌───────────────────────────────────────────────────┐   │
│  │                Domain Layer                        │   │
│  │  ─────────────────────────────────────────────── │   │
│  │  • Entities & Aggregates                          │   │
│  │  • Value Objects (Money, StockQuantity)            │   │
│  │  • Business Invariants                            │   │
│  │  • ZERO infrastructure dependencies               │   │
│  └───────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────┘
```

### Bounded Contexts

| Context | Responsibility |
|---------|----------------|
| 🏪 **Catalog** | Products, categories, stock management |
| 🛒 **Basket** | Shopping cart for guests & registered users |
| 📦 **Ordering** | Order creation, lifecycle & state machine |
| 💳 **Payment** | Tokenized payment processing |
| 🔐 **Identity** | Authentication, JWT, role management |

---

## 📁 Project Structure

```
ECommerce/
├── 📄 ECommerce.sln
└── src/
    ├── 🔵 ECommerce.Domain/           ← Pure domain (zero dependencies)
    │   ├── Common/                     BaseEntity, BaseAuditableEntity, IRepository
    │   ├── Catalog/                    Product, Category, Money, StockQuantity
    │   ├── Basket/                     Basket, BasketItem
    │   ├── Ordering/                   Order, OrderItem, OrderStatus
    │   ├── Payment/                    PaymentRecord, PaymentStatus
    │   └── Identity/                   AppUser, UserRole
    │
    ├── 🟢 ECommerce.Application/      ← CQRS Commands & Queries
    │   ├── Common/                     IApplicationDbContext, Behaviors
    │   ├── Catalog/Commands|Queries/   CreateProduct, GetProducts...
    │   ├── Basket/Commands|Queries/    AddToBasket, GetBasket...
    │   ├── Ordering/Commands|Queries/  CreateOrder, GetOrders...
    │   ├── Payment/Commands/           ProcessPayment
    │   └── Identity/Commands|Queries/  Register, Login, RefreshToken...
    │
    ├── 🟡 ECommerce.Persistence/      ← EF Core + PostgreSQL
    │   ├── Context/                    ApplicationDbContext
    │   ├── Configurations/             Fluent API (6 entity configs)
    │   ├── Interceptors/               AuditAndSoftDeleteInterceptor
    │   └── Repositories/               Generic Repository<T>
    │
    ├── 🟠 ECommerce.Infrastructure/   ← External concerns
    │   ├── Identity/                   JwtService
    │   ├── Payment/                    StubPaymentService
    │   └── Logging/                    Serilog configuration
    │
    └── 🔴 ECommerce.API/              ← Orchestrator (no business logic)
        ├── Controllers/                Catalog, Basket, Order, Payment, Auth
        ├── Controllers/Admin/          AdminController (role-gated)
        ├── Middlewares/                ExceptionHandlingMiddleware
        └── Extensions/                 DI registrations
```

### Dependency Flow

```
Domain         ← (independent — ZERO references)
Application    ← Domain
Persistence    ← Domain, Application
Infrastructure ← Domain, Application
API            ← Application, Persistence, Infrastructure
```

> ⚠️ **API never references Domain directly.** Everything flows through Application interfaces.

---

## 🚀 Getting Started

### Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0+ |
| [PostgreSQL](https://www.postgresql.org/) | 14+ |
| [Docker](https://www.docker.com/) | Optional |

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/EminBozkaya/E_Commerce.git
cd E_Commerce

# 2. Update the connection string in appsettings
# src/ECommerce.API/appsettings.Development.json

# 3. Apply migrations
dotnet ef migrations add InitialCreate \
  --project src/ECommerce.Persistence \
  --startup-project src/ECommerce.API

dotnet ef database update \
  --project src/ECommerce.Persistence \
  --startup-project src/ECommerce.API

# 4. Run the application
dotnet run --project src/ECommerce.API

# 5. Open Swagger UI
# https://localhost:5001/swagger
```

### Docker (PostgreSQL)

```bash
docker run -d \
  --name ecommerce-db \
  -e POSTGRES_USER=postgresUser \
  -e POSTGRES_PASSWORD=postgresPassword \
  -e POSTGRES_DB=ECommerceDb \
  -p 5432:5432 \
  postgres:16-alpine
```

---

## 📡 API Reference

### 🔓 Public Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/catalog/products` | List all products |
| `GET` | `/api/catalog/products/{id}` | Get product details |
| `GET` | `/api/catalog/categories` | List all categories |
| `GET` | `/api/basket` | View basket |
| `POST` | `/api/basket/items` | Add item to basket |
| `DELETE` | `/api/basket/items/{productId}` | Remove item from basket |
| `POST` | `/api/order` | Create order from basket |
| `POST` | `/api/payment` | Process payment |

### 🔑 Auth Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register new account |
| `POST` | `/api/auth/login` | Login (sets httpOnly cookie) |
| `POST` | `/api/auth/refresh` | Refresh token rotation |
| `POST` | `/api/auth/logout` | Clear auth cookies |

### 🛡️ Admin Endpoints `[Authorize(Roles = "Admin")]`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/admin/products` | Create product |
| `PUT` | `/api/admin/products` | Update product |
| `DELETE` | `/api/admin/products/{id}` | Delete product (soft) |
| `PUT` | `/api/admin/products/stock` | Update stock |
| `POST` | `/api/admin/categories` | Create category |
| `GET` | `/api/admin/users` | List all users |
| `GET` | `/api/admin/orders` | List all orders |

---

## 🔒 Security

This project follows **enterprise security standards** with defense-in-depth:

| Feature | Implementation |
|---------|----------------|
| 🔐 Authentication | JWT via **httpOnly secure cookies** |
| 🔄 Token Rotation | Refresh tokens with **mandatory rotation** |
| 🚫 No localStorage | Tokens **never** stored in localStorage |
| 💳 Payment Safety | **Tokenized payments only** — no card data stored |
| 🛡️ Rate Limiting | IP-based, 100 req/min per client |
| 🌐 CORS | Strict **whitelist** policy |
| 🔑 Password Hashing | PBKDF2 with SHA-256 (100K iterations) |
| ⏱️ Short-lived Tokens | 15-minute access tokens, zero clock skew |
| 🗑️ Soft Delete | Data **never** physically deleted |

---

## 🧪 Tech Stack

<table>
<tr>
<td align="center"><b>Runtime</b></td>
<td align="center"><b>ORM</b></td>
<td align="center"><b>Database</b></td>
<td align="center"><b>CQRS</b></td>
<td align="center"><b>Validation</b></td>
<td align="center"><b>Logging</b></td>
<td align="center"><b>Docs</b></td>
</tr>
<tr>
<td align="center">.NET 10</td>
<td align="center">EF Core 10</td>
<td align="center">PostgreSQL</td>
<td align="center">MediatR</td>
<td align="center">FluentValidation</td>
<td align="center">Serilog</td>
<td align="center">Swagger</td>
</tr>
</table>

---

## 🗺️ Roadmap

- [x] Solution architecture & project setup
- [x] Domain models with DDD (entities, value objects, aggregates)
- [x] EF Core with PostgreSQL (Code First, Fluent API)
- [x] Soft-delete & audit interceptor
- [x] CQRS with MediatR (Commands & Queries)
- [x] JWT authentication (httpOnly cookies)
- [x] Admin panel endpoints
- [x] Global exception handling
- [x] Rate limiting & CORS
- [ ] Email confirmation flow
- [ ] Pagination & filtering
- [ ] Redis caching layer
- [ ] Real payment provider integration (Iyzico / Stripe)
- [ ] React frontend (SPA)
- [ ] Docker Compose for full stack
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Unit & integration tests

---

<p align="center">
  <sub>Built with ❤️ using .NET 10 • Clean Architecture • CQRS • DDD</sub>
</p>
