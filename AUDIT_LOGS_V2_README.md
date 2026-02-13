# Audit Logs V2 - Implementation Guide

## Overview

This is a **completely separate** implementation of audit logging (V2) built from scratch alongside the existing V1 system. Both systems run independently without interfering with each other.

## What's New in V2

### Key Features
✅ **Timeline-Based UI** - Git-like checkpoint visualization  
✅ **Before/After Comparison** - Visual diff of changed fields  
✅ **HttpContext Integration** - Automatic capture of user & IP data  
✅ **Advanced Filtering** - Date range, entity type, user filters  
✅ **Optimized Performance** - Indexed queries, projection-based fetching  
✅ **Separate Database Table** - Independent from V1 (AuditLogsV2)  

---

## Architecture

### Backend (.NET 8)

#### 1. Domain Layer (`Inventory.Domain`)
```
Entities/
  └── AuditLogV2.cs         # New entity with GUID, JSON fields
Interfaces/
  └── IAuditLogV2Repository.cs
```

#### 2. Infrastructure Layer (`Inventory.Infrastructure`)
```
Repositories/
  └── AuditLogV2Repository.cs     # EF Core repository
Data/
  └── ApplicationDbContext.cs     # DbSet<AuditLogV2> added
Migrations/
  └── AddAuditLogV2Table.sql      # Manual migration script
```

#### 3. Application Layer (`Inventory.Application`)
```
DTOs/
  ├── AuditLogV2Dto.cs
  ├── AuditLogTimelineDto.cs
  ├── AuditLogDetailDto.cs
  └── AuditLogTimelineGroupDto.cs
  
Interfaces/
  └── IAuditLogV2Service.cs
  
Services/
  └── AuditLogV2Service.cs        # HttpContext-aware logging
```

#### 4. API Layer (`Inventory.API`)
```
Controllers/
  └── AuditLogsV2Controller.cs    # /api/auditlogs/v2/*
```

---

### Frontend (React + Vite)

```
pages/
  └── AuditLogsV2.jsx              # Main page with state management

components/auditv2/
  ├── AuditTimeline.jsx            # Vertical timeline view
  ├── AuditCheckpoint.jsx          # Individual timeline items
  ├── AuditDetailPanel.jsx         # Slide-in detail panel
  └── AuditFilterSidebar.jsx       # Filter controls

lib/
  └── api.js                       # auditLogsV2API methods
```

---

## API Endpoints

### 1. Get Timeline
```http
GET /api/auditlogs/v2/timeline?fromDate=2026-02-01&toDate=2026-02-13
```
**Response:**
```json
[
  {
    "date": "2026-02-13",
    "logs": [
      {
        "id": "guid",
        "message": "Abhishek updated Product Laptop",
        "timestamp": "2026-02-13T10:30:00",
        "userName": "Abhishek",
        "entityName": "Product",
        "actionType": "Updated"
      }
    ]
  }
]
```

### 2. Get Detail
```http
GET /api/auditlogs/v2/{id}
```
**Response:**
```json
{
  "id": "guid",
  "message": "Abhishek updated Product Laptop",
  "timestamp": "2026-02-13T10:30:00",
  "userName": "Abhishek",
  "userId": "user-guid",
  "actionType": "Updated",
  "entityName": "Product",
  "entityId": "product-guid",
  "oldValues": { "price": 50000, "quantity": 10 },
  "newValues": { "price": 45000, "quantity": 15 },
  "changedFields": ["price", "quantity"],
  "ipAddress": "192.168.1.100"
}
```

### 3. Filter
```http
GET /api/auditlogs/v2/filter?fromDate=2026-02-01&entityName=Product
```

---

## Database Schema

### AuditLogsV2 Table
```sql
CREATE TABLE AuditLogsV2 (
    Id             UNIQUEIDENTIFIER PRIMARY KEY,
    UserId         NVARCHAR(100) NOT NULL,
    UserName       NVARCHAR(200) NOT NULL,
    ActionType     NVARCHAR(100) NOT NULL,  -- Created, Updated, Deleted
    EntityName     NVARCHAR(100) NOT NULL,  -- Product, Inventory
    EntityId       NVARCHAR(100) NOT NULL,
    Message        NVARCHAR(500) NOT NULL,
    Timestamp      DATETIME2 DEFAULT GETUTCDATE(),
    OldValues      NVARCHAR(MAX) NULL,      -- JSON serialized
    NewValues      NVARCHAR(MAX) NULL,      -- JSON serialized
    ChangedFields  NVARCHAR(MAX) NULL,      -- JSON array
    IpAddress      NVARCHAR(50) NULL
);

-- Indexes
CREATE INDEX IX_AuditLogsV2_Timestamp ON AuditLogsV2(Timestamp DESC);
CREATE INDEX IX_AuditLogsV2_UserId ON AuditLogsV2(UserId);
CREATE INDEX IX_AuditLogsV2_EntityId ON AuditLogsV2(EntityId);
CREATE INDEX IX_AuditLogsV2_EntityName ON AuditLogsV2(EntityName);
```

---

## How It Works

### 1. Automatic Audit Logging (Backend)

The `AuditLogV2Service` automatically captures:
- **User Info** - From JWT claims in HttpContext
- **IP Address** - From HttpContext.Connection
- **Changed Fields** - By comparing old/new JSON objects

Example in ProductService:
```csharp
// V1 logging (unchanged)
await CreateAuditLog(userId, "Update", "Product", ...);

// V2 logging (new)
await _auditLogV2Service.LogAsync(
    actionType: "Updated",
    entityName: "Product",
    entityId: product.Id.ToString(),
    oldValues: new { Name = oldProduct.Name, Price = oldProduct.Price },
    newValues: new { Name = product.Name, Price = product.Price },
    message: $"Updated Product {product.Name}"
);
```

### 2. Timeline UI (Frontend)

**Features:**
- Vertical timeline with checkpoints
- Grouped by date (Today, Yesterday, specific dates)
- Color-coded action types
- Click to expand details

**User Flow:**
1. User opens `/audit-logs-v2`
2. Sees timeline of all activities
3. Clicks a checkpoint → Detail panel slides in
4. Views before/after comparison with highlighted changes
5. Changes visible side-by-side

---

## Setup Instructions

### 1. Database Migration

**Option A: Using EF Core**
```bash
cd server/Inventory.API
dotnet ef migrations add AddAuditLogV2Table
dotnet ef database update
```

**Option B: Manual SQL Script**
```bash
# Run the SQL script manually
server/Inventory.Infrastructure/Migrations/AddAuditLogV2Table.sql
```

### 2. Backend Setup

Already configured! The following are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IAuditLogV2Repository, AuditLogV2Repository>();
builder.Services.AddScoped<IAuditLogV2Service, AuditLogV2Service>();
builder.Services.AddHttpContextAccessor(); // For HttpContext access
```

### 3. Frontend Setup

Routes already added to `App.jsx`:
```jsx
<Route path="/audit-logs-v2" element={<ProtectedRoute><AuditLogsV2 /></ProtectedRoute>} />
```

Navigation dropdown in `Layout.jsx`:
- Audit Logs v1 → `/audit-logs`
- Audit Logs v2 → `/audit-logs-v2`

---

## Testing the Implementation

### 1. Create Product
```http
POST /api/products
{
  "name": "Laptop",
  "description": "Gaming Laptop",
  "quantity": 10,
  "price": 50000
}
```
✅ Creates audit log: "Created Product Laptop"

### 2. Update Product
```http
PUT /api/products/{id}
{
  "name": "Laptop",
  "description": "Gaming Laptop",
  "quantity": 15,
  "price": 45000
}
```
✅ Creates audit log with before/after comparison

### 3. View Timeline
1. Navigate to `/audit-logs-v2`
2. See checkpoints:
   - ● 10:30 AM - Created Product Laptop
   - ● 11:00 AM - Updated Product Laptop

### 4. View Details
1. Click on "Updated Product Laptop"
2. Panel opens showing:
   - **Before:** Price: 50000, Quantity: 10
   - **After:** Price: 45000, Quantity: 15
   - **Highlighted:** price, quantity

---

## Performance Optimizations

### Backend
1. **Indexed Queries** - Timestamp, UserId, EntityId, EntityName
2. **Projection** - `Select()` only needed fields for timeline
3. **Async/Await** - All operations are async
4. **No Tracking** - `AsNoTracking()` for read-only queries

### Frontend
1. **Lazy Loading** - Detail panel loads on click
2. **Optimistic UI** - Instant feedback on interactions
3. **Grouped Timeline** - Reduced render complexity
4. **Conditional Rendering** - Detail panel only when needed

---

## Differences: V1 vs V2

| Feature | V1 | V2 |
|---------|----|----|
| **Entity** | `AuditLog` | `AuditLogV2` |
| **Table** | `AuditLogs` | `AuditLogsV2` |
| **UI** | Simple table | Timeline with checkpoints |
| **Data Capture** | Manual userId/IP | Auto from HttpContext |
| **Comparison** | No | Yes (Before/After) |
| **Filtering** | Basic | Advanced (Date, Entity, User) |
| **Performance** | Standard | Optimized with indexes |
| **Route** | `/audit-logs` | `/audit-logs-v2` |

---

## File Structure

```
server/
  └── Inventory.Domain/
      └── Entities/
          └── AuditLogV2.cs ✨
      └── Interfaces/
          └── IAuditLogV2Repository.cs ✨
  
  └── Inventory.Infrastructure/
      └── Repositories/
          └── AuditLogV2Repository.cs ✨
      └── Data/
          └── ApplicationDbContext.cs (modified)
      └── Migrations/
          └── AddAuditLogV2Table.sql ✨
  
  └── Inventory.Application/
      └── DTOs/
          ├── AuditLogV2Dto.cs ✨
          ├── AuditLogTimelineDto.cs ✨
          ├── AuditLogDetailDto.cs ✨
          └── AuditLogTimelineGroupDto.cs ✨
      └── Interfaces/
          └── IAuditLogV2Service.cs ✨
      └── Services/
          ├── AuditLogV2Service.cs ✨
          └── ProductService.cs (modified)
  
  └── Inventory.API/
      └── Controllers/
          └── AuditLogsV2Controller.cs ✨
      └── Program.cs (modified)

client/
  └── src/
      └── pages/
          └── AuditLogsV2.jsx ✨
      └── components/auditv2/
          ├── AuditTimeline.jsx ✨
          ├── AuditCheckpoint.jsx ✨
          ├── AuditDetailPanel.jsx ✨
          └── AuditFilterSidebar.jsx ✨
      └── lib/
          └── api.js (modified)
      └── App.jsx (modified)
```

✨ = New file  
(modified) = Existing file updated

---

## Future Enhancements

- [ ] Pagination for large datasets
- [ ] Real-time updates using SignalR
- [ ] Virtual scrolling for performance
- [ ] Export to CSV/PDF
- [ ] Search functionality
- [ ] Restore from checkpoint
- [ ] Audit log retention policies

---

## Troubleshooting

### Issue: Migration fails
**Solution:** Stop the running API and retry, or use the manual SQL script.

### Issue: HttpContext is null
**Solution:** Ensure `builder.Services.AddHttpContextAccessor()` is in Program.cs

### Issue: No data in timeline
**Solution:** Perform some product operations first (Create/Update/Delete)

### Issue: Detail panel not showing
**Solution:** Check console for API errors, verify JWT token is valid

---

## Summary

🎯 **V2 is production-ready** with:
- Complete separation from V1
- Timeline-based UI with checkpoint visualization
- Before/after comparison with highlighted changes
- HttpContext-based automatic data capture
- Optimized database queries with indexes
- Advanced filtering capabilities

Both V1 and V2 work independently - you can use them side-by-side!
