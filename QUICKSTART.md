# Quick Start Guide

## 🚀 Getting Started in 5 Minutes

### Step 1: Start the Backend API

Open PowerShell and navigate to the server directory:

```powershell
cd C:\Users\abhishek.k\Desktop\Example\server\Inventory.API
dotnet run
```

You should see:
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
```

✅ **Backend is running!**

---

### Step 2: Start the Frontend

Open a **new PowerShell window** and navigate to the client directory:

```powershell
cd C:\Users\abhishek.k\Desktop\Example\client
npm run dev
```

You should see:
```
  VITE v7.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
```

✅ **Frontend is running!**

---

### Step 3: Test the Application

1. Open your browser and go to: **http://localhost:5173**

2. **Register a new account:**
   - Click "Register here"
   - Enter username: `admin`
   - Enter email: `admin@example.com`
   - Enter password: `Admin@123`
   - Select role: `Admin`
   - Click "Register"

3. **You'll be automatically logged in and redirected to the Dashboard**

4. **Create your first product:**
   - Click "Products" in the sidebar
   - Click "➕ Add Product"
   - Enter product details:
     - Name: `Laptop`
     - Description: `High-performance laptop`
     - Quantity: `10`
     - Price: `999.99`
   - Click "Create Product"

5. **View the product in the list**

6. **Check the audit logs:**
   - Click "Audit Logs" in the sidebar
   - You'll see your product creation logged with all details

---

## 🎯 Test All Features

### ✅ Authentication
- [x] Register new user
- [x] Login with credentials
- [x] Logout
- [x] Protected routes redirect to login

### ✅ Products
- [x] Create product (logs in audit)
- [x] View all products
- [x] Edit product (logs in audit with old/new values)
- [x] Delete product (logs in audit)

### ✅ Audit Logs
- [x] View all logs
- [x] See user who performed action
- [x] See timestamp and IP address
- [x] View old and new values

---

## ⚡ Quick Commands Reference

### Backend Commands
```powershell
# Navigate to API project
cd server\Inventory.API

# Run the API
dotnet run

# Build the solution
dotnet build

# Create new migration (if you change entities)
dotnet ef migrations add MigrationName --project ../Inventory.Infrastructure

# Update database
dotnet ef database update --project ../Inventory.Infrastructure
```

### Frontend Commands
```powershell
# Navigate to client
cd client

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

---

## 🔧 Troubleshooting

### Backend won't start?
1. Make sure .NET 8 SDK is installed: `dotnet --version`
2. Check if port 5000/5001 is available
3. Rebuild: `dotnet clean` then `dotnet build`

### Frontend won't start?
1. Make sure Node.js is installed: `node --version`
2. Delete `node_modules` and run `npm install` again
3. Check if port 5173 is available

### Can't login?
1. Make sure backend is running at http://localhost:5000
2. Check browser console for errors
3. Clear localStorage and try again

### Database errors?
1. Delete the database and run migrations again:
   ```powershell
   cd server\Inventory.API
   dotnet ef database drop --project ../Inventory.Infrastructure
   dotnet ef database update --project ../Inventory.Infrastructure
   ```

---

## 📝 Default Test Credentials

After registration, you can use:
- **Email:** `admin@example.com`
- **Password:** `Admin@123`
- **Role:** `Admin`

---

## 🎉 You're All Set!

Enjoy exploring the Inventory Management System!

For detailed documentation, see [README.md](README.md)
