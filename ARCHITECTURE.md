# System Architecture Documentation

## 🏗️ Architecture Overview

This Inventory Management System follows **Clean Architecture** principles with clear separation of concerns across multiple layers.

---

## Backend Architecture

### N-Tier Architecture Layers

```
┌─────────────────────────────────────────┐
│         Inventory.API (Presentation)    │
│  - Controllers                          │
│  - Middleware                           │
│  - Program.cs (Startup Configuration)  │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│      Inventory.Application (Business)   │
│  - Services (Business Logic)            │
│  - DTOs (Data Transfer Objects)        │
│  - Interfaces                           │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│    Inventory.Infrastructure (Data)      │
│  - DbContext                            │
│  - Repositories                         │
│  - Unit of Work                         │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│       Inventory.Domain (Core)           │
│  - Entities                             │
│  - Interfaces                           │
└─────────────────────────────────────────┘
```

---

## Layer Responsibilities

### 1. **Domain Layer** (Core)
**Purpose:** Contains enterprise-wide business rules and entities

**Components:**
- `Entities/User.cs` - User entity with authentication properties
- `Entities/Product.cs` - Product entity with inventory details
- `Entities/AuditLog.cs` - Audit trail entity
- `Interfaces/IRepository.cs` - Generic repository interface
- `Interfaces/IUserRepository.cs` - User-specific operations
- `Interfaces/IProductRepository.cs` - Product-specific operations
- `Interfaces/IAuditLogRepository.cs` - Audit log operations
- `Interfaces/IUnitOfWork.cs` - Unit of Work pattern

**Dependencies:** None (Clean Architecture - innermost layer)

---

### 2. **Application Layer** (Business Logic)
**Purpose:** Contains application-specific business rules

**Components:**

**DTOs (Data Transfer Objects):**
- `RegisterDto` - User registration data
- `LoginDto` - User login credentials
- `LoginResponseDto` - Authentication response
- `ProductDto` - Product data transfer
- `CreateProductDto` - Product creation data
- `UpdateProductDto` - Product update data
- `AuditLogDto` - Audit log data transfer

**Services:**
- `AuthService` - Handles user registration, login, password hashing
- `ProductService` - Manages CRUD operations for products
- `AuditLogService` - Retrieves and filters audit logs
- `JwtService` - Generates JWT tokens

**Interfaces:**
- `IAuthService` - Authentication operations contract
- `IProductService` - Product operations contract
- `IAuditLogService` - Audit log operations contract
- `IJwtService` - JWT generation contract

**Dependencies:** Domain Layer

---

### 3. **Infrastructure Layer** (Data Access)
**Purpose:** Implements data access and external dependencies

**Components:**

**Data:**
- `ApplicationDbContext` - EF Core DbContext with entity configurations

**Repositories:**
- `Repository<T>` - Generic repository implementation
- `UserRepository` - User-specific database operations
- `ProductRepository` - Product-specific database operations
- `AuditLogRepository` - Audit log database operations
- `UnitOfWork` - Coordinates multiple repositories and transactions

**Dependencies:** Domain Layer

---

### 4. **API Layer** (Presentation)
**Purpose:** Handles HTTP requests and responses

**Components:**

**Controllers:**
- `AuthController` - Handles /api/auth/* endpoints
  - POST /register - User registration
  - POST /login - User authentication
  
- `ProductsController` - Handles /api/products/* endpoints (Protected)
  - GET / - Get all products
  - GET /{id} - Get product by ID
  - POST / - Create product
  - PUT /{id} - Update product
  - DELETE /{id} - Delete product
  
- `AuditLogsController` - Handles /api/auditlogs/* endpoints (Protected)
  - GET / - Get all logs
  - GET /user/{userId} - Get logs by user
  - GET /action/{action} - Get logs by action

**Middleware:**
- `ErrorHandlingMiddleware` - Global exception handler

**Configuration:**
- `Program.cs` - Dependency injection, JWT configuration, CORS setup

**Dependencies:** Application Layer, Infrastructure Layer

---

## Design Patterns Used

### 1. **Repository Pattern**
- Abstracts data access logic
- Provides clean API for domain objects
- Located in: `Inventory.Domain/Interfaces` and `Inventory.Infrastructure/Repositories`

### 2. **Unit of Work Pattern**
- Coordinates multiple repositories
- Ensures transactional consistency
- Located in: `Inventory.Domain/Interfaces/IUnitOfWork.cs`

### 3. **Dependency Injection**
- All services registered in `Program.cs`
- Constructor injection throughout
- Promotes loose coupling

### 4. **DTO Pattern**
- Separates internal entities from API contracts
- Prevents over-posting attacks
- Located in: `Inventory.Application/DTOs`

### 5. **Service Layer Pattern**
- Encapsulates business logic
- Called by controllers
- Located in: `Inventory.Application/Services`

---

## Security Architecture

### Authentication Flow

```
1. User → POST /api/auth/register
   ↓
2. AuthController → AuthService.RegisterAsync()
   ↓
3. AuthService → Hash password with BCrypt
   ↓
4. AuthService → Save user to database via UnitOfWork
   ↓
5. JwtService → Generate JWT token
   ↓
6. Return token + user details to client
```

### Authorization Flow

```
1. Client → Request with JWT in Authorization header
   ↓
2. JWT Middleware → Validate token signature
   ↓
3. JWT Middleware → Extract claims (userId, role)
   ↓
4. [Authorize] attribute → Check authentication
   ↓
5. Controller → Access user claims from HttpContext
```

---

## Audit Logging Architecture

### Automatic Audit Trail

Every product operation triggers audit logging:

**Create Product:**
```
1. ProductService.CreateProductAsync()
   ↓
2. Create product entity
   ↓
3. CreateAuditLog(userId, "Create", "Product", productId, null, product, ipAddress)
   ↓
4. Save both product and audit log in same transaction
```

**Update Product:**
```
1. ProductService.UpdateProductAsync()
   ↓
2. Get existing product (oldProduct)
   ↓
3. Update product properties
   ↓
4. CreateAuditLog(userId, "Update", "Product", productId, oldProduct, product, ipAddress)
   ↓
5. Save changes
```

**Delete Product:**
```
1. ProductService.DeleteProductAsync()
   ↓
2. Get existing product
   ↓
3. CreateAuditLog(userId, "Delete", "Product", productId, product, null, ipAddress)
   ↓
4. Delete product
```

**Audit Log Storage:**
- Old values stored as JSON string
- New values stored as JSON string
- User ID, timestamp, IP address tracked
- Linked to User entity via foreign key

---

## Frontend Architecture

### Component Hierarchy

```
App.jsx (Router)
├── Navbar (Global navigation)
├── Sidebar (Side menu)
└── Routes
    ├── Login (Public)
    ├── Register (Public)
    └── Protected Routes
        ├── Dashboard
        ├── Products
        ├── AddProduct
        ├── EditProduct
        └── AuditLogs
```

### Service Layer

```
services/
├── api.js (Axios instance with interceptors)
├── authService.js (Authentication operations)
├── productService.js (Product CRUD operations)
└── auditLogService.js (Audit log retrieval)
```

### State Management

- **LocalStorage** for token and user data
- **Component State (useState)** for UI state
- **React Router** for navigation state

### API Communication Flow

```
1. Component → Service function (e.g., productService.getAllProducts())
   ↓
2. Service → api.js (Axios instance)
   ↓
3. Axios Interceptor → Add JWT token to headers
   ↓
4. HTTP Request → Backend API
   ↓
5. Response → Return data to component
   ↓
6. Component → Update state and re-render
```

---

## Database Schema

```sql
Users
├── Id (GUID, PK)
├── Username (VARCHAR, NOT NULL)
├── Email (VARCHAR, UNIQUE, NOT NULL)
├── PasswordHash (VARCHAR, NOT NULL)
├── Role (VARCHAR, NOT NULL)
└── CreatedAt (DATETIME, DEFAULT GETUTCDATE())

Products
├── Id (GUID, PK)
├── Name (VARCHAR, NOT NULL)
├── Description (VARCHAR)
├── Quantity (INT, NOT NULL)
├── Price (DECIMAL(18,2), NOT NULL)
├── CreatedAt (DATETIME, DEFAULT GETUTCDATE())
└── UpdatedAt (DATETIME, DEFAULT GETUTCDATE())

AuditLogs
├── Id (GUID, PK)
├── UserId (GUID, FK → Users.Id)
├── Action (VARCHAR, NOT NULL)
├── EntityName (VARCHAR, NOT NULL)
├── EntityId (VARCHAR, NOT NULL)
├── OldValues (TEXT, NULLABLE)
├── NewValues (TEXT, NULLABLE)
├── Timestamp (DATETIME, DEFAULT GETUTCDATE())
└── IpAddress (VARCHAR, NOT NULL)
```

---

## Configuration Management

### Backend Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string"
  },
  "JwtSettings": {
    "SecretKey": "Secret key for JWT signing",
    "Issuer": "Token issuer",
    "Audience": "Token audience",
    "ExpireMinutes": "Token expiration time"
  }
}
```

### Frontend Configuration (api.js)

```javascript
const API_URL = 'http://localhost:5000/api';
```

---

## SOLID Principles Applied

### **S - Single Responsibility**
- Each service has one responsibility
- Controllers only handle HTTP concerns
- Repositories only handle data access

### **O - Open/Closed**
- Generic repository can be extended without modification
- Service interfaces allow different implementations

### **L - Liskov Substitution**
- Repository implementations can be swapped
- Service implementations can be replaced

### **I - Interface Segregation**
- Specific repository interfaces (IUserRepository, IProductRepository)
- Client-specific service interfaces

### **D - Dependency Inversion**
- Controllers depend on IService interfaces
- Services depend on IRepository interfaces
- Configuration via dependency injection

---

## Error Handling Strategy

### Backend
1. **Try-Catch in Services** - Business logic errors
2. **Global Middleware** - Unhandled exceptions
3. **Custom Exceptions** - Domain-specific errors
4. **HTTP Status Codes** - Proper REST semantics

### Frontend
1. **Try-Catch in Service Calls** - API errors
2. **Error State** - Display error messages
3. **Axios Interceptors** - Global error handling
4. **401 Redirect** - Unauthorized handling

---

## Scalability Considerations

### Current Architecture Supports:
- ✅ Horizontal scaling (stateless API)
- ✅ Database connection pooling
- ✅ Async/await for non-blocking I/O
- ✅ Clean separation for microservices migration

### Future Enhancements:
- Add caching layer (Redis)
- Implement CQRS pattern
- Add message queue (RabbitMQ/Azure Service Bus)
- Split into microservices
- Add API Gateway

---

## Testing Strategy

### Unit Tests (Recommended)
- Test service logic in isolation
- Mock repository dependencies
- Test JWT generation

### Integration Tests (Recommended)
- Test API endpoints
- Test database operations
- Test authentication flow

### E2E Tests (Recommended)
- Test complete user workflows
- Test frontend → backend integration

---

## Documentation

This architecture enables:
- ✅ Easy maintenance
- ✅ Clear separation of concerns
- ✅ Testability
- ✅ Flexibility for future changes
- ✅ Scalability
- ✅ Security best practices

---

**For implementation details, see individual code files.**
