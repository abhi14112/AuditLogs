# 🌐 API Documentation

## Base URL
```
http://localhost:5000/api
```

---

## 🔓 Public Endpoints

### Register User
**POST** `/auth/register`

Creates a new user account.

**Request Body:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "role": "User" | "Admin"
}
```

**Success Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "string",
  "email": "string",
  "role": "string"
}
```

**Error Response:** `400 Bad Request`
```json
{
  "message": "User with this email already exists"
}
```

---

### Login User
**POST** `/auth/login`

Authenticates a user and returns JWT token.

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Success Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "string",
  "email": "string",
  "role": "string"
}
```

**Error Response:** `401 Unauthorized`
```json
{
  "message": "Invalid email or password"
}
```

---

## 🔒 Protected Endpoints

All endpoints below require JWT authentication.

**Authorization Header:**
```
Authorization: Bearer <your_jwt_token>
```

---

## 📦 Product Endpoints

### Get All Products
**GET** `/products`

Retrieves all products.

**Success Response:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Laptop",
    "description": "High-performance laptop",
    "quantity": 10,
    "price": 999.99,
    "createdAt": "2026-02-12T10:30:00Z",
    "updatedAt": "2026-02-12T10:30:00Z"
  }
]
```

---

### Get Product By ID
**GET** `/products/{id}`

Retrieves a specific product.

**URL Parameters:**
- `id` (GUID) - Product ID

**Success Response:** `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Laptop",
  "description": "High-performance laptop",
  "quantity": 10,
  "price": 999.99,
  "createdAt": "2026-02-12T10:30:00Z",
  "updatedAt": "2026-02-12T10:30:00Z"
}
```

**Error Response:** `404 Not Found`
```json
{
  "message": "Product not found"
}
```

---

### Create Product
**POST** `/products`

Creates a new product and logs the action.

**Request Body:**
```json
{
  "name": "Laptop",
  "description": "High-performance laptop",
  "quantity": 10,
  "price": 999.99
}
```

**Success Response:** `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Laptop",
  "description": "High-performance laptop",
  "quantity": 10,
  "price": 999.99,
  "createdAt": "2026-02-12T10:30:00Z",
  "updatedAt": "2026-02-12T10:30:00Z"
}
```

**Audit Log:** Automatically creates audit log entry with action "Create"

---

### Update Product
**PUT** `/products/{id}`

Updates an existing product and logs the changes.

**URL Parameters:**
- `id` (GUID) - Product ID

**Request Body:**
```json
{
  "name": "Gaming Laptop",
  "description": "Updated description",
  "quantity": 15,
  "price": 1299.99
}
```

**Success Response:** `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Gaming Laptop",
  "description": "Updated description",
  "quantity": 15,
  "price": 1299.99,
  "createdAt": "2026-02-12T10:30:00Z",
  "updatedAt": "2026-02-12T11:45:00Z"
}
```

**Error Response:** `404 Not Found`
```json
{
  "message": "Product not found"
}
```

**Audit Log:** Creates audit log with action "Update", stores old and new values as JSON

---

### Delete Product
**DELETE** `/products/{id}`

Deletes a product and logs the action.

**URL Parameters:**
- `id` (GUID) - Product ID

**Success Response:** `204 No Content`

**Error Response:** `404 Not Found`
```json
{
  "message": "Product not found"
}
```

**Audit Log:** Creates audit log with action "Delete", stores old values as JSON

---

## 📋 Audit Log Endpoints

### Get All Audit Logs
**GET** `/auditlogs`

Retrieves all audit logs ordered by timestamp (newest first).

**Success Response:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "username": "admin",
    "action": "Create",
    "entityName": "Product",
    "entityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "oldValues": null,
    "newValues": "{\"Name\":\"Laptop\",\"Description\":\"High-performance\",\"Quantity\":10,\"Price\":999.99}",
    "timestamp": "2026-02-12T10:30:00Z",
    "ipAddress": "127.0.0.1"
  }
]
```

---

### Get Logs By User ID
**GET** `/auditlogs/user/{userId}`

Retrieves audit logs for a specific user.

**URL Parameters:**
- `userId` (GUID) - User ID

**Success Response:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "username": "admin",
    "action": "Update",
    "entityName": "Product",
    "entityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "oldValues": "{\"Name\":\"Laptop\",\"Price\":999.99}",
    "newValues": "{\"Name\":\"Gaming Laptop\",\"Price\":1299.99}",
    "timestamp": "2026-02-12T11:45:00Z",
    "ipAddress": "127.0.0.1"
  }
]
```

---

### Get Logs By Action
**GET** `/auditlogs/action/{action}`

Retrieves audit logs filtered by action type.

**URL Parameters:**
- `action` (string) - Action type: `Create`, `Update`, or `Delete`

**Example:** `/auditlogs/action/Create`

**Success Response:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "username": "admin",
    "action": "Create",
    "entityName": "Product",
    "entityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "oldValues": null,
    "newValues": "{\"Name\":\"Mouse\",\"Price\":29.99}",
    "timestamp": "2026-02-12T10:30:00Z",
    "ipAddress": "127.0.0.1"
  }
]
```

---

## 🔑 JWT Token Structure

### Token Claims
```json
{
  "sub": "user-guid-here",
  "email": "user@example.com",
  "name": "username",
  "role": "User",
  "jti": "unique-token-id",
  "exp": 1707745200,
  "iss": "InventoryAPI",
  "aud": "InventoryClient"
}
```

### Extracting User Information

In controllers, access user information via claims:

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var email = User.FindFirst(ClaimTypes.Email)?.Value;
var username = User.FindFirst(ClaimTypes.Name)?.Value;
var role = User.FindFirst(ClaimTypes.Role)?.Value;
```

---

## 📊 HTTP Status Codes

| Code | Description | Usage |
|------|-------------|-------|
| 200 | OK | Successful GET, PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation errors, duplicate records |
| 401 | Unauthorized | Invalid/missing JWT token |
| 404 | Not Found | Resource doesn't exist |
| 500 | Internal Server Error | Unhandled exceptions |

---

## 🛡️ Error Response Format

All error responses follow this format:

```json
{
  "message": "Error description here"
}
```

---

## 🧪 Example API Calls

### Using cURL

**Register:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@example.com",
    "password": "Admin@123",
    "role": "Admin"
  }'
```

**Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "Admin@123"
  }'
```

**Get Products (with token):**
```bash
curl -X GET http://localhost:5000/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Create Product:**
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "name": "Laptop",
    "description": "High-performance laptop",
    "quantity": 10,
    "price": 999.99
  }'
```

---

### Using JavaScript (Axios)

**Register:**
```javascript
import axios from 'axios';

const response = await axios.post('http://localhost:5000/api/auth/register', {
  username: 'admin',
  email: 'admin@example.com',
  password: 'Admin@123',
  role: 'Admin'
});

const { token } = response.data;
```

**Get Products:**
```javascript
const response = await axios.get('http://localhost:5000/api/products', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const products = response.data;
```

---

## 🔄 CORS Configuration

The API allows requests from:
- `http://localhost:5173` (Vite dev server)
- `http://localhost:3000` (Alternative React dev server)

CORS is configured to allow:
- ✅ All HTTP methods (GET, POST, PUT, DELETE)
- ✅ All headers
- ✅ Credentials (cookies, authorization headers)

---

## 📝 Notes

1. **DateTime Format:** All timestamps are in UTC ISO 8601 format
2. **GUID Format:** All IDs are UUID/GUID strings
3. **Decimal Precision:** Prices use decimal(18,2) format
4. **Token Expiration:** JWT tokens expire after 60 minutes (configurable)
5. **Automatic Audit Logging:** All product operations are automatically logged

---

## 🚀 Testing Tools

Recommended tools for API testing:
- **Postman** - GUI-based API client
- **Thunder Client** - VS Code extension
- **cURL** - Command-line tool
- **Browser DevTools** - Network tab

---

**For integration examples, see the React services in `client/src/services/`**
