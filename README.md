# Order Management API

A production-style RESTful Web API built with **ASP.NET Core 8** to demonstrate backend development concepts such as Entity Framework Core, PostgreSQL, JWT authentication, Redis caching, Docker, optimistic concurrency, and unit testing.

This project was built as a practical backend project to strengthen real-world **.NET and C# development** skills.

## Tech Stack

* **Language:** C#
* **Framework:** ASP.NET Core 8 Web API
* **ORM:** Entity Framework Core 8
* **Database:** PostgreSQL
* **Authentication:** JWT Bearer Authentication
* **Caching:** Redis with StackExchange.Redis
* **Testing:** xUnit, Moq
* **Containerization:** Docker
* **API Documentation:** Swagger / OpenAPI
* **Version Control:** Git & GitHub

## Features

* RESTful Order Management APIs
* Create and retrieve orders
* DTO-based request and response models
* Entity Framework Core with PostgreSQL
* Repository and Service layers
* Dependency Injection
* JWT-based authentication
* Role-based authorization
* Global exception handling middleware
* Optimistic concurrency handling
* Redis caching
* Asynchronous programming with `async` / `await`
* LINQ and EF Core query optimization
* Unit testing with xUnit and Moq
* Docker containerization
* Swagger / OpenAPI documentation

## Project Structure

```text
OrderManagementApi/

+-- Controllers/       # Handles HTTP requests
+-- Services/          # Business logic
+-- Repositories/      # Database operations
+-- Models/
|   +-- DTOs/          # Request/response models
|   +-- Entities/      # Database entities
+-- Data/              # EF Core DbContext
+-- Middleware/        # Global exception handling
+-- Migrations/        # EF Core migrations
+-- OrderManagementApi.Tests/
+-- Dockerfile
+-- Program.cs
+-- appsettings.json
+-- README.md
```

## Architecture

The API follows a layered architecture:

**Controller -> Service -> Repository -> PostgreSQL**

Additional components:

* **JWT Authentication & Authorization** for securing APIs
* **Redis** for caching
* **Middleware** for global exception handling
* **Entity Framework Core** for database access
* **Dependency Injection** for managing application dependencies

## API Endpoints

### Authentication

| Method | Endpoint             | Description                             | Authentication |
| ------ | -------------------- | --------------------------------------- | -------------- |
| POST   | `/api/Auth/register` | Register a new user                     | No             |
| POST   | `/api/Auth/login`    | Authenticate user and receive JWT token | No             |

### Orders

| Method | Endpoint           | Description        | Authentication |
| ------ | ------------------ | ------------------ | -------------- |
| POST   | `/api/Orders`      | Create a new order | Required       |
| GET    | `/api/Orders/{id}` | Get an order by ID | Required       |
| PUT    | `/api/Orders/{id}` | Update an order    | Required       |
| DELETE | `/api/Orders/{id}` | Delete an order    | Required       |

## Authentication

The API uses **JWT Bearer Authentication**.

The authentication flow is:

```text
Register
   |
   v
Login
   |
   v
JWT Token
   |
   v
Send Token with API Request
   |
   v
JWT Authentication
   |
   v
Authorization
```

Example HTTP header:

```text
Authorization: Bearer <your-jwt-token>
```

JWT claims are used to identify the authenticated user and apply role-based authorization.

## Database

The project uses **PostgreSQL** with **Entity Framework Core**.

The database layer includes:

* `Order` entity
* `AppUser` entity
* `ApplicationDbContext`
* EF Core migrations
* Repository pattern
* Asynchronous database operations
* LINQ queries
* Optimistic concurrency using a `Version` field

### Entity Framework Core

EF Core is responsible for:

* Mapping C# entities to PostgreSQL tables
* Querying data using LINQ
* Tracking entity changes
* Saving changes to the database
* Managing database migrations

## Redis Caching

The API uses **Redis** with `StackExchange.Redis` for caching.

Example cache key:

```text
order:{id}
```

The order retrieval flow uses caching to reduce unnecessary database queries:

```text
GET Order
   |
   v
Check Redis
   |
   +---- Cache Hit ----> Return Cached Data
   |
   +---- Cache Miss
             |
             v
       Query PostgreSQL
             |
             v
        Store in Redis
             |
             v
        Return Response
```

Redis is used as a caching layer between the API and PostgreSQL.

## Optimistic Concurrency

The project implements optimistic concurrency using an order `Version` field.

When an order is updated or deleted:

```text
Client sends current Version
          |
          v
API checks database Version
          |
     +----+----+
     |         |
   Match    Mismatch
     |         |
     v         v
 Update      409 Conflict
```

A `409 Conflict` response is returned when the supplied version does not match the current database version.

This helps prevent one client from accidentally overwriting another client's changes.

## Global Exception Handling

The API uses custom middleware for global exception handling.

Instead of handling unexpected exceptions separately in every controller, exceptions are handled centrally by middleware.

```text
Controller
    |
    v
Service
    |
    v
Exception
    |
    v
Global Exception Middleware
    |
    v
Consistent HTTP Error Response
```

This keeps controllers cleaner and provides consistent error responses.

## Dependency Injection

The application uses ASP.NET Core's built-in **Dependency Injection** system.

Example dependency flow:

```text
Controller
    |
    v
IOrderService
    |
    v
IOrderRepository
    |
    v
ApplicationDbContext
```

Dependencies are registered in `Program.cs` and injected into the required classes.

## Unit Testing

Unit tests are implemented using:

* **xUnit** for the testing framework
* **Moq** for mocking dependencies

The tests focus mainly on the service layer.

Examples of tested scenarios include:

* Creating a valid order
* Rejecting invalid quantity
* Rejecting invalid price
* Retrieving an existing order
* Handling missing orders
* Updating orders
* Deleting orders
* Handling concurrency conflicts
* Validating service behavior

Run the tests with:

```powershell
dotnet test
```

## Docker

The application can be containerized using **Docker**.

The Docker setup includes:

```text
Docker
   |
   +-- ASP.NET Core API Container
   |
   +-- Redis Container
   |
   +-- PostgreSQL
```

The API and Redis containers communicate through a Docker network.

The API container connects to PostgreSQL running on the host machine and Redis through the Docker network.

Build the Docker image:

```powershell
docker build -t order-management-api .
```

Run Redis:

```powershell
docker run --name order-redis --network order-network -p 6379:6379 -d redis
```

Run the API:

```powershell
docker run --name order-api --network order-network -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Docker order-management-api
```

## Swagger / OpenAPI

Swagger is included for API documentation and testing.

When the application is running, Swagger provides an interactive interface to:

* View available endpoints
* View request and response models
* Test API requests
* Provide JWT authentication
* Inspect API responses

## Running Locally

### Prerequisites

Install the following:

* .NET 8 SDK
* PostgreSQL
* Redis
* Git
* Docker Desktop (optional)

### Clone the Repository

```powershell
git clone https://github.com/shyamedison2408/order-management-api.git
cd order-management-api
```

### Configure Secrets

Sensitive values such as database passwords and JWT signing keys should not be committed to Git.

For local development, use **.NET User Secrets**:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-postgresql-connection-string>"
dotnet user-secrets set "Jwt:Key" "<your-jwt-secret>"
```

### Apply Database Migrations

```powershell
dotnet ef database update
```

### Run the API

```powershell
dotnet run
```

The API will start on the configured local development URL.

Open Swagger in the browser to test the API.

## Running Tests

Run all unit tests using:

```powershell
dotnet test
```

## Git Workflow

The project uses Git for source control.

Typical workflow:

```text
Create Feature
     |
     v
Implement Code
     |
     v
Run Tests
     |
     v
Git Status
     |
     v
Git Add
     |
     v
Git Commit
     |
     v
Git Push
```

## Key Backend Concepts Demonstrated

This project demonstrates practical knowledge of:

* ASP.NET Core Web API
* C# and OOP
* REST API design
* HTTP status codes
* Dependency Injection
* Middleware
* Repository and Service patterns
* Entity Framework Core
* LINQ
* PostgreSQL
* JWT Authentication
* Role-based Authorization
* Redis caching
* Optimistic Concurrency
* Async/Await
* Unit Testing
* Mocking with Moq
* Docker
* Git and GitHub
* Swagger / OpenAPI

## Future Improvements

Possible future improvements include:

* Azure deployment
* GitHub Actions CI/CD
* Integration testing
* Health checks
* Structured logging
* API versioning
* Pagination and filtering
* Docker Compose
* Production secret management

## Author

**Shyam**

.NET Backend Developer

GitHub: `https://github.com/shyamedison2408`
