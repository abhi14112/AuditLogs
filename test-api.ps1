# Test API Endpoints Script
$baseUrl = "https://localhost:7001/api"

Write-Host "Testing Inventory Management API Endpoints..." -ForegroundColor Cyan
Write-Host ""

# Test 1: Register a new user
Write-Host "1. Testing Registration Endpoint..." -ForegroundColor Yellow
try {
    $registerBody = @{
        username = "testuser"
        email = "test@example.com"
        password = "Test@123"
    } | ConvertTo-Json

    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/auth/register" `
        -Method Post `
        -Body $registerBody `
        -ContentType "application/json" `
        -ErrorAction Stop `
        -SkipCertificateCheck
    
    $token = $registerResponse.token
    Write-Host "[OK] Registration successful!" -ForegroundColor Green
    Write-Host "  Token received: $($token.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host ""
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "[OK] User already exists, trying login..." -ForegroundColor Yellow
        
        # Test 2: Login with existing user
        Write-Host "2. Testing Login Endpoint..." -ForegroundColor Yellow
        try {
            $loginBody = @{
                email = "test@example.com"
                password = "Test@123"
            } | ConvertTo-Json

            $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" `
                -Method Post `
                -Body $loginBody `
                -ContentType "application/json" `
                -ErrorAction Stop `
                -SkipCertificateCheck
            
            $token = $loginResponse.token
            Write-Host "[OK] Login successful!" -ForegroundColor Green
            Write-Host "  Token: $($token.Substring(0, 20))..." -ForegroundColor Gray
            Write-Host ""
        } catch {
            Write-Host "[FAIL] Login failed: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "[FAIL] Registration failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
        exit 1
    }
}

# Test 3: Get Products (Protected)
Write-Host "3. Testing GET Products Endpoint (with auth)..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $products = Invoke-RestMethod -Uri "$baseUrl/products" `
        -Method Get `
        -Headers $headers `
        -ErrorAction Stop `
        -SkipCertificateCheck
    
    Write-Host "[OK] Products retrieved successfully!" -ForegroundColor Green
    Write-Host "  Total products: $($products.Count)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "[FAIL] Get products failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Test 4: Create Product
Write-Host "4. Testing POST Product Endpoint..." -ForegroundColor Yellow
try {
    $productBody = @{
        name = "Test Product $(Get-Date -Format 'HHmmss')"
        description = "This is a test product created by API test script"
        quantity = 100
        price = 29.99
    } | ConvertTo-Json

    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $newProduct = Invoke-RestMethod -Uri "$baseUrl/products" `
        -Method Post `
        -Body $productBody `
        -ContentType "application/json" `
        -Headers $headers `
        -ErrorAction Stop `
        -SkipCertificateCheck
    
    $productId = $newProduct.id
    Write-Host "[OK] Product created successfully!" -ForegroundColor Green
    Write-Host "  Product ID: $productId" -ForegroundColor Gray
    Write-Host "  Name: $($newProduct.name)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "[FAIL] Create product failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Test 5: Update Product
if ($productId) {
    Write-Host "5. Testing PUT Product Endpoint..." -ForegroundColor Yellow
    try {
        $updateBody = @{
            name = "Updated Test Product"
            description = "This product was updated"
            quantity = 150
            price = 39.99
        } | ConvertTo-Json

        $headers = @{
            "Authorization" = "Bearer $token"
        }
        
        $updatedProduct = Invoke-RestMethod -Uri "$baseUrl/products/$productId" `
            -Method Put `
            -Body $updateBody `
            -ContentType "application/json" `
            -Headers $headers `
            -ErrorAction Stop `
            -SkipCertificateCheck
        
        Write-Host "[OK] Product updated successfully!" -ForegroundColor Green
        Write-Host "  Name: $($updatedProduct.name)" -ForegroundColor Gray
        Write-Host "  Quantity: $($updatedProduct.quantity)" -ForegroundColor Gray
        Write-Host ""
    } catch {
        Write-Host "[FAIL] Update product failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# Test 6: Get Audit Logs
Write-Host "6. Testing GET Audit Logs Endpoint..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $auditLogs = Invoke-RestMethod -Uri "$baseUrl/auditlogs" `
        -Method Get `
        -Headers $headers `
        -ErrorAction Stop `
        -SkipCertificateCheck
    
    Write-Host "[OK] Audit logs retrieved successfully!" -ForegroundColor Green
    Write-Host "  Total logs: $($auditLogs.Count)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "[FAIL] Get audit logs failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
}

# Test 7: Delete Product
if ($productId) {
    Write-Host "7. Testing DELETE Product Endpoint..." -ForegroundColor Yellow
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
        }
        
        Invoke-RestMethod -Uri "$baseUrl/products/$productId" `
            -Method Delete `
            -Headers $headers `
            -ErrorAction Stop `
            -SkipCertificateCheck
        
        Write-Host "[OK] Product deleted successfully!" -ForegroundColor Green
        Write-Host ""
    } catch {
        Write-Host "[FAIL] Delete product failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Error: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# Test 8: Test Unauthorized Access
Write-Host "8. Testing Unauthorized Access..." -ForegroundColor Yellow
try {
    Invoke-RestMethod -Uri "$baseUrl/products" `
        -Method Get `
        -ErrorAction Stop `
        -SkipCertificateCheck
    
    Write-Host "X Expected 401 Unauthorized but request succeeded!" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "OK Correctly returned 401 Unauthorized!" -ForegroundColor Green
    } else {
        Write-Host "X Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "API Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
