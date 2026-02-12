# Inventory Management System

A production-ready full-stack Inventory Management System built with **React** (frontend) and **ASP.NET Core 8 Web API** (backend) with JWT authentication, SQL Server database, and comprehensive audit logging.

## 🚀 Features

### Backend (ASP.NET Core 8)
- **N-Tier Architecture** (Domain, Application, Infrastructure, API)
- **Clean Architecture** with SOLID principles
- **Repository Pattern** and **Unit of Work**
- **JWT Authentication** with BCrypt password hashing
- **SQL Server LocalDB** with Entity Framework Core
- **Automatic Audit Logging** for all CRUD operations
- **Role-based Authorization**
- **Global Error Handling Middleware**
- **CORS Configuration** for React app

### Frontend (React + Vite)
- **Modern React** with Hooks
- **TailwindCSS** for styling
- **React Router v6** for navigation
- **Axios** with JWT interceptors
- **Protected Routes**
- **Responsive UI Design**
- Clean dashboard with sidebar navigation

### Database Tables
1. **Users** - User authentication and management
2. **Products** - Inventory product management
3. **AuditLogs** - Complete audit trail with old/new values

## 📁 Project Structure

```
Root/
├── server/                          # Backend (.NET 8)
│   ├── Inventory.API/               # Web API Layer
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── Inventory.Application/       # Business Logic Layer
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── Inventory.Domain/            # Domain Layer
│   │   ├── Entities/
│   │   └── Interfaces/
│   └── Inventory.Infrastructure/    # Data Access Layer
│       ├── Data/
│       └── Repositories/
└── client/                          # Frontend (React)
    ├── src/
    │   ├── components/
    │   ├── pages/
    │   └── services/
    └── package.json
```

## 🛠️ Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js** (v18 or higher) - [Download](https://nodejs.org/)
- **SQL Server LocalDB** (comes with Visual Studio)

## 📦 Installation & Setup

### Backend Setup

1. Navigate to the server directory:
```powershell
cd server
```

2. Restore NuGet packages:
```powershell
dotnet restore
```

3. Apply database migrations:
```powershell
cd Inventory.API
dotnet ef database update --project ../Inventory.Infrastructure --startup-project .
```

4. Run the API:
```powershell
dotnet run
```

The API will run at: `https://localhost:5001` or `http://localhost:5000`

### Frontend Setup

1. Navigate to the client directory:
```powershell
cd client
```

2. Install dependencies:
```powershell
npm install
```

3. Start the development server:
```powershell
npm run dev
```

The React app will run at: `http://localhost:5173`

## 🔑 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Products (Protected)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Audit Logs (Protected)
- `GET /api/auditlogs` - Get all audit logs
- `GET /api/auditlogs/user/{userId}` - Get logs by user
- `GET /api/auditlogs/action/{action}` - Get logs by action

## 🎯 Usage Guide

### 1. Register a New User
- Navigate to `http://localhost:5173/register`
- Fill in username, email, password
- Select role (User or Admin)
- Click "Register"

### 2. Login
- Navigate to `http://localhost:5173/login`
- Enter email and password
- Click "Login"

### 3. Manage Products
- Click "Products" in the sidebar
- Add new products with "Add Product" button
- Edit or delete existing products
- All actions are automatically logged

### 4. View Audit Logs
- Click "Audit Logs" in the sidebar
- View complete history of all actions
- See user, timestamp, action type, and changes
- Expand "View Changes" to see old and new values

## 🔒 Security Features

- **Password Hashing** using BCrypt
- **JWT Token Authentication** with configurable expiration
- **Token stored in localStorage**
- **Axios interceptors** for automatic token attachment
- **Protected routes** requiring authentication
- **IP Address logging** for audit trail
- **CORS configuration** for specific origins

## 🗄️ Database Configuration

Connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## 📝 Environment Variables

### Backend (appsettings.json)
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123456789",
    "Issuer": "InventoryAPI",
    "Audience": "InventoryClient",
    "ExpireMinutes": "60"
  }
}
```

### Frontend (src/services/api.js)
```javascript
const API_URL = 'http://localhost:5000/api';
```

## 🧪 Testing the Application

1. **Register** a new user account
2. **Login** with your credentials
3. **Create** a product (check audit logs)
4. **Update** the product (check audit logs)
5. **Delete** the product (check audit logs)
6. **View** audit logs to see complete history

## 🎨 UI Features

- **Clean Dashboard** with quick actions
- **Responsive Design** with TailwindCSS
- **Sidebar Navigation** with active state
- **Data Tables** for products and audit logs
- **Form Validation** on all inputs
- **Loading States** for async operations
- **Error Handling** with user feedback

## 📊 Audit Logging

Every product action automatically creates an audit log with:
- **User ID** and **Username**
- **Action Type** (Create, Update, Delete)
- **Entity Name** (Product)
- **Entity ID** (Product GUID)
- **Old Values** (JSON before update/delete)
- **New Values** (JSON after create/update)
- **Timestamp** (UTC)
- **IP Address** of the client

## 🚀 Production Deployment Considerations

1. **Change JWT Secret Key** to a strong random value
2. **Configure CORS** for production domain
3. **Use Environment Variables** for sensitive data
4. **Enable HTTPS** in production
5. **Configure SQL Server** instead of LocalDB
6. **Add Logging** (Serilog, NLog, etc.)
7. **Implement Rate Limiting**
8. **Add Input Validation** on all endpoints
9. **Set up CI/CD** pipeline
10. **Configure Application Insights** or monitoring

## 🛡️ Best Practices Implemented

- ✅ Clean Architecture
- ✅ SOLID Principles
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ DTO Pattern
- ✅ Async/Await everywhere
- ✅ Error handling middleware
- ✅ Service layer separation
- ✅ No business logic in controllers
- ✅ Proper HTTP status codes

## 📚 Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server LocalDB
- BCrypt.Net-Next
- System.IdentityModel.Tokens.Jwt

### Frontend
- React 18
- Vite
- TailwindCSS
- React Router v6
- Axios

## 🤝 Contributing

This is a production-ready template. Feel free to:
- Add more features
- Improve UI/UX
- Add unit tests
- Enhance security
- Add more audit log filters

## 📄 License

MIT License - feel free to use this project for learning or production.

## 👨‍💻 Developer

Built with ❤️ using Clean Architecture and best practices.

---

**Happy Coding! 🎉**
