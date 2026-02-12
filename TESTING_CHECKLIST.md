# ✅ Testing Checklist

Use this checklist to verify all features are working correctly.

---

## 🔧 Pre-Testing Setup

- [ ] Backend is running at `http://localhost:5000`
- [ ] Frontend is running at `http://localhost:5173`
- [ ] Database is created and migrations are applied
- [ ] Browser is open at `http://localhost:5173`
- [ ] Browser DevTools console is open (F12)

---

## 1️⃣ User Registration Tests

### Test Case 1.1: Successful Registration
- [ ] Navigate to `/register`
- [ ] Enter username: `testuser`
- [ ] Enter email: `test@example.com`
- [ ] Enter password: `Test@123`
- [ ] Confirm password: `Test@123`
- [ ] Select role: `User`
- [ ] Click "Register"
- [ ] ✅ Should redirect to Dashboard
- [ ] ✅ Should see welcome message with username
- [ ] ✅ Token should be in localStorage (check DevTools → Application → Local Storage)

### Test Case 1.2: Duplicate Email
- [ ] Logout
- [ ] Try registering with same email
- [ ] ✅ Should show error: "User with this email already exists"

### Test Case 1.3: Password Mismatch
- [ ] Enter different passwords in password fields
- [ ] ✅ Should show error: "Passwords do not match"

---

## 2️⃣ User Login Tests

### Test Case 2.1: Successful Login
- [ ] Navigate to `/login`
- [ ] Enter email: `test@example.com`
- [ ] Enter password: `Test@123`
- [ ] Click "Login"
- [ ] ✅ Should redirect to Dashboard
- [ ] ✅ Should see navbar with username

### Test Case 2.2: Invalid Credentials
- [ ] Try login with wrong password
- [ ] ✅ Should show error: "Invalid email or password"

### Test Case 2.3: Logout
- [ ] Click "Logout" button in navbar
- [ ] ✅ Should redirect to login page
- [ ] ✅ Token should be removed from localStorage

---

## 3️⃣ Protected Routes Tests

### Test Case 3.1: Access Protected Route When Not Logged In
- [ ] Clear localStorage
- [ ] Try to visit `/dashboard` directly
- [ ] ✅ Should redirect to `/login`

### Test Case 3.2: Access Protected Route When Logged In
- [ ] Login successfully
- [ ] Visit `/products`
- [ ] ✅ Should display products page

---

## 4️⃣ Product CRUD Tests

### Test Case 4.1: Create Product
- [ ] Click "Products" in sidebar
- [ ] Click "➕ Add Product"
- [ ] Enter name: `Laptop`
- [ ] Enter description: `High-performance laptop for developers`
- [ ] Enter quantity: `10`
- [ ] Enter price: `999.99`
- [ ] Click "Create Product"
- [ ] ✅ Should redirect to products list
- [ ] ✅ New product should appear in table
- [ ] ✅ Check audit logs - should see "Create" action

### Test Case 4.2: View All Products
- [ ] Click "Products" in sidebar
- [ ] ✅ Should display table with all products
- [ ] ✅ Should show: Name, Description, Quantity, Price, Actions

### Test Case 4.3: Update Product
- [ ] Click "Edit" on a product
- [ ] Change name to: `Gaming Laptop`
- [ ] Change price to: `1299.99`
- [ ] Click "Update Product"
- [ ] ✅ Should redirect to products list
- [ ] ✅ Should show updated values
- [ ] ✅ Check audit logs - should see "Update" action
- [ ] ✅ Expand "View Changes" - should see old and new values

### Test Case 4.4: Delete Product
- [ ] Click "Delete" on a product
- [ ] Click "OK" in confirmation dialog
- [ ] ✅ Product should disappear from list
- [ ] ✅ Check audit logs - should see "Delete" action
- [ ] ✅ Old values should be stored in audit log

### Test Case 4.5: Cancel Operations
- [ ] Go to Add Product, click "Cancel"
- [ ] ✅ Should return to products list
- [ ] Go to Edit Product, click "Cancel"
- [ ] ✅ Should return to products list

---

## 5️⃣ Audit Log Tests

### Test Case 5.1: View All Logs
- [ ] Click "Audit Logs" in sidebar
- [ ] ✅ Should display table with all audit logs
- [ ] ✅ Should show: Timestamp, User, Action, Entity, IP Address, Details

### Test Case 5.2: Log Details
- [ ] Click "View Changes" on an "Update" log entry
- [ ] ✅ Should show "Old Values" section
- [ ] ✅ Should show "New Values" section
- [ ] ✅ Values should be in JSON format

### Test Case 5.3: Log Timestamps
- [ ] Check timestamps on logs
- [ ] ✅ Should be in readable format (e.g., "2/12/2026, 10:30:15 AM")
- [ ] ✅ Should be ordered by newest first

### Test Case 5.4: Action Badges
- [ ] ✅ "Create" actions should have green badge
- [ ] ✅ "Update" actions should have blue badge
- [ ] ✅ "Delete" actions should have red badge

---

## 6️⃣ Navigation Tests

### Test Case 6.1: Sidebar Navigation
- [ ] Click "Dashboard" - should navigate to dashboard
- [ ] Click "Products" - should navigate to products
- [ ] Click "Audit Logs" - should navigate to audit logs
- [ ] ✅ Active page should be highlighted in sidebar

### Test Case 6.2: Navbar
- [ ] ✅ "Inventory System" logo should link to dashboard
- [ ] ✅ Should display current username
- [ ] ✅ Logout button should be visible

### Test Case 6.3: Dashboard Quick Actions
- [ ] Click "➕ Add New Product" - should go to add product page
- [ ] Click "📋 View All Products" - should go to products page
- [ ] Click "🔍 Check Audit Logs" - should go to audit logs page

---

## 7️⃣ Validation Tests

### Test Case 7.1: Product Form Validation
- [ ] Go to Add Product
- [ ] Try submitting empty form
- [ ] ✅ Should show required field errors
- [ ] Try entering negative quantity
- [ ] ✅ Should not allow negative values

### Test Case 7.2: Login Form Validation
- [ ] Go to Login
- [ ] Try submitting empty form
- [ ] ✅ Should show required field errors
- [ ] Enter invalid email format
- [ ] ✅ Should show email validation error

---

## 8️⃣ Error Handling Tests

### Test Case 8.1: API Connection Error
- [ ] Stop the backend server
- [ ] Try to fetch products
- [ ] ✅ Should show error message

### Test Case 8.2: 401 Unauthorized
- [ ] Clear localStorage token
- [ ] Try to create a product (by directly calling API)
- [ ] ✅ Should redirect to login

### Test Case 8.3: Network Error Display
- [ ] ✅ All API errors should display in red alert boxes
- [ ] ✅ Success messages should be clear

---

## 9️⃣ UI/UX Tests

### Test Case 9.1: Responsive Design
- [ ] Resize browser window
- [ ] ✅ Layout should adapt to different sizes
- [ ] ✅ Sidebar should be visible on desktop
- [ ] ✅ Tables should be scrollable on small screens

### Test Case 9.2: Loading States
- [ ] Watch for "Loading..." messages
- [ ] ✅ Should show while fetching data
- [ ] ✅ Should hide after data loads

### Test Case 9.3: Button States
- [ ] Click submit buttons
- [ ] ✅ Should show "Loading..." or disabled state
- [ ] ✅ Should prevent double-click

---

## 🔟 Security Tests

### Test Case 10.1: JWT Token
- [ ] Check localStorage after login
- [ ] ✅ Token should be present
- [ ] Login to jwt.io and decode token
- [ ] ✅ Should contain userId, email, role claims

### Test Case 10.2: Password Hashing
- [ ] Check database Users table
- [ ] ✅ PasswordHash should be hashed (long string)
- [ ] ✅ Plain password should NOT be visible

### Test Case 10.3: Authorization Headers
- [ ] Check Network tab in DevTools
- [ ] Make an API request to products
- [ ] ✅ Should include `Authorization: Bearer <token>` header

---

## 1️⃣1️⃣ Database Tests

### Test Case 11.1: User Creation
- [ ] Register a new user
- [ ] Check `Users` table in SQL Server
- [ ] ✅ User should exist with hashed password
- [ ] ✅ CreatedAt should be set

### Test Case 11.2: Product Creation
- [ ] Create a product
- [ ] Check `Products` table
- [ ] ✅ Product should exist with all fields
- [ ] ✅ CreatedAt and UpdatedAt should be set

### Test Case 11.3: Audit Log Creation
- [ ] Perform any product action
- [ ] Check `AuditLogs` table
- [ ] ✅ Log entry should exist
- [ ] ✅ Should have UserId, Action, EntityName, Timestamp
- [ ] ✅ Old/New values should be JSON strings

---

## 1️⃣2️⃣ End-to-End Workflow Test

### Complete User Journey
1. [ ] Register new user `admin@test.com`
2. [ ] Login with new account
3. [ ] Navigate to Dashboard
4. [ ] Create 3 products:
   - [ ] `Laptop` - $999.99, Qty: 10
   - [ ] `Mouse` - $29.99, Qty: 50
   - [ ] `Keyboard` - $79.99, Qty: 30
5. [ ] Update one product (change price)
6. [ ] Delete one product
7. [ ] View all products - should see 2 remaining
8. [ ] Check Audit Logs - should see 5 entries (3 creates, 1 update, 1 delete)
9. [ ] Expand an update log - verify old/new values
10. [ ] Logout
11. [ ] Login again - should work
12. [ ] Data should persist

✅ **All steps completed successfully = System is working perfectly!**

---

## 📊 Test Results Summary

| Category | Total Tests | Passed | Failed |
|----------|-------------|--------|--------|
| Authentication | 7 | ___ | ___ |
| Protected Routes | 2 | ___ | ___ |
| Product CRUD | 5 | ___ | ___ |
| Audit Logs | 4 | ___ | ___ |
| Navigation | 3 | ___ | ___ |
| Validation | 2 | ___ | ___ |
| Error Handling | 3 | ___ | ___ |
| UI/UX | 3 | ___ | ___ |
| Security | 3 | ___ | ___ |
| Database | 3 | ___ | ___ |
| E2E Workflow | 1 | ___ | ___ |
| **TOTAL** | **36** | ___ | ___ |

---

## 🐛 Bug Reporting Template

If you find any issues:

```
**Bug Description:**
[Describe what went wrong]

**Steps to Reproduce:**
1. 
2. 
3. 

**Expected Behavior:**
[What should happen]

**Actual Behavior:**
[What actually happened]

**Error Message (if any):**
[Copy error from console]

**Screenshot:**
[Attach if applicable]
```

---

**Happy Testing! 🎯**
