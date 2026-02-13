# Quick Start Guide - Audit Logs V2

## 🚀 Running the Application

### Prerequisites
- .NET 8 SDK
- Node.js (v18+)
- SQL Server

---

## Step 1: Database Setup

### Option 1: Automatic Migration (Recommended)
```bash
# Navigate to API project
cd server/Inventory.API

# Stop any running API instance first
# Then run migration
dotnet ef database update
```

### Option 2: Manual SQL Execution
If the API is running and migration fails:
1. Open SQL Server Management Studio
2. Connect to your database
3. Run the script: `server/Inventory.Infrastructure/Migrations/AddAuditLogV2Table.sql`

---

## Step 2: Start Backend

```bash
cd server/Inventory.API
dotnet run
```

**Backend should start at:** `https://localhost:7237`

---

## Step 3: Start Frontend

```bash
cd client
npm install  # Only first time
npm run dev
```

**Frontend should start at:** `http://localhost:5173`

---

## Step 4: Test the System

### 1. Login
- Navigate to `http://localhost:5173/login`
- Login with existing credentials or register new user

### 2. Create a Product
- Go to Products page
- Click "Add Product"
- Fill in: Name, Description, Quantity, Price
- Click Save
- ✅ **This creates an audit log entry**

### 3. Update a Product
- Click edit on any product
- Change price or quantity
- Click Update
- ✅ **This creates an audit log entry with before/after values**

### 4. View Audit Logs V2
- Click "Audit Logs" dropdown in header
- Select "Audit Logs v2"
- You should see:
  - Timeline view with checkpoints
  - Created/Updated activities
  - Grouped by date

### 5. View Details
- Click on any checkpoint
- Right panel slides in showing:
  - User info, timestamp, IP address
  - Before/After comparison
  - Highlighted changed fields

### 6. Filter Logs
- Use left sidebar to filter:
  - **Date Range:** From/To dates
  - **Entity Type:** Product, Inventory, User
  - **Quick Filters:** Today, Last 7 days, Last 30 days
- Click "Apply Filters"

---

## 🧪 Testing Scenarios

### Scenario 1: Product Creation Flow
```
1. Create Product "Laptop" - Price: 50000, Qty: 10
2. Go to Audit Logs v2
3. See: "● Created Product Laptop"
4. Click on it
5. Verify: NewValues shows all fields, OldValues is null
```

### Scenario 2: Product Update Flow
```
1. Update "Laptop" - Change Price to 45000, Qty to 15
2. Go to Audit Logs v2
3. See: "● Updated Product Laptop"
4. Click on it
5. Verify: 
   - Before: Price: 50000, Quantity: 10
   - After: Price: 45000, Quantity: 15
   - Changed Fields: ["Price", "Quantity"] highlighted
```

### Scenario 3: Product Deletion Flow
```
1. Delete product "Mouse"
2. Go to Audit Logs v2
3. See: "● Deleted Product Mouse"
4. Click on it
5. Verify: OldValues shows deleted data, NewValues is null
```

### Scenario 4: Filtering
```
1. Create/Update/Delete multiple products
2. Go to Audit Logs v2
3. Select "Today" quick filter
4. See only today's activities
5. Change Entity Type to "Product"
6. See only product-related activities
```

---

## 📊 Expected Data Structure

### Timeline View
```
Today
  ● 10:30 AM - Abhishek created Product Laptop
  ● 11:00 AM - Abhishek updated Product Laptop
  ● 12:30 PM - Admin deleted Product Mouse

Yesterday
  ● 09:15 AM - Abhishek created Product Keyboard
```

### Detail Panel (Update Example)
```
Message: Abhishek updated Product Laptop
User: Abhishek
Action: Updated
Entity: Product
Timestamp: Feb 13, 2026, 11:00:00 AM
IP Address: 192.168.1.100

Changes (2 fields modified):
┌─────────────┬──────────┬──────────┐
│ Field       │ Before   │ After    │
├─────────────┼──────────┼──────────┤
│ Price       │ 50000    │ 45000    │ ⚠️ Changed
│ Quantity    │ 10       │ 15       │ ⚠️ Changed
│ Name        │ Laptop   │ Laptop   │
│ Description │ Gaming   │ Gaming   │
└─────────────┴──────────┴──────────┘
```

---

## 🎨 UI Features

### Timeline Checkpoint Colors
- 🟢 **Green** - Created
- 🔵 **Blue** - Updated
- 🔴 **Red** - Deleted
- 🟣 **Purple** - Login
- ⚪ **Gray** - Logout

### Interactive Elements
- **Hover** - Checkpoint opacity increases
- **Click** - Detail panel slides in from right
- **Active** - Blue border on selected checkpoint
- **Backdrop** - Click outside to close detail panel

---

## 🔍 API Testing (Optional)

### Using curl or Postman

#### 1. Get Timeline
```bash
curl -X GET "https://localhost:7237/api/auditlogs/v2/timeline" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### 2. Get Detail
```bash
curl -X GET "https://localhost:7237/api/auditlogs/v2/{LOG_ID}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### 3. Filter
```bash
curl -X GET "https://localhost:7237/api/auditlogs/v2/filter?entityName=Product" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 🐛 Common Issues

### 1. "Cannot connect to server"
**Solution:** Ensure backend is running at `https://localhost:7237`

### 2. "401 Unauthorized"
**Solution:** Login again to refresh JWT token

### 3. "No audit logs found"
**Solution:** Perform some product operations first

### 4. Migration error: "Table already exists"
**Solution:** Table already created, skip migration

### 5. Detail panel not showing data
**Solution:** 
- Open browser console (F12)
- Check for API errors
- Verify log ID is valid

---

## ✨ Key Differences from V1

| Aspect | V1 | V2 |
|--------|----|----|
| **URL** | `/audit-logs` | `/audit-logs-v2` |
| **View** | Table | Timeline |
| **Comparison** | ❌ | ✅ Before/After |
| **Auto-capture** | Manual | HttpContext |
| **Filters** | Basic | Advanced |
| **Visual** | Simple | Rich UI |

---

## 📈 Performance Tips

1. **Use Date Filters** - Narrow down timeline for faster loading
2. **Entity Filter** - Filter by specific entity type
3. **Pagination** - (Future enhancement) for very large datasets

---

## 🎯 Success Criteria

You've successfully implemented V2 if:
- ✅ Can see timeline view at `/audit-logs-v2`
- ✅ Product operations create audit logs
- ✅ Clicking checkpoint shows detail panel
- ✅ Before/After comparison is visible
- ✅ Changed fields are highlighted
- ✅ Filters work correctly
- ✅ V1 still works independently at `/audit-logs`

---

## 📞 Need Help?

Check the main documentation: `AUDIT_LOGS_V2_README.md`

Happy auditing! 🎉
