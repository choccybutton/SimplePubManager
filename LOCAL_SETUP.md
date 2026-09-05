# SimplePubManager - Local Development Setup Guide

## Prerequisites

- .NET 8 SDK
- PostgreSQL 14+ (running on localhost:5432)
- Node.js 18+ & npm
- Git

## Step 1: Database Setup

### 1a. Verify PostgreSQL is Running

```bash
# Check if PostgreSQL is running
psql --version

# If not running, start PostgreSQL (Windows)
# or
brew services start postgresql  # macOS
# or
sudo systemctl start postgresql  # Linux
```

### 1b. Create Database

```bash
# Connect to PostgreSQL
psql -U postgres

# Inside psql prompt:
CREATE DATABASE simplepubmanager;
CREATE USER simplepubmanager WITH PASSWORD 'simplepubmanager';
ALTER ROLE simplepubmanager WITH CREATEDB;
ALTER DATABASE simplepubmanager OWNER TO simplepubmanager;
\q
```

### 1c. Verify Connection

```bash
psql -U simplepubmanager -d simplepubmanager -h localhost
# Should connect successfully
\q
```

## Step 2: Configure Backend

### 2a. Update Database Connection String

Edit `src/SimplePubManager.Api/appsettings.Development.json` if it exists, or edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=simplepubmanager;Username=simplepubmanager;Password=simplepubmanager"
}
```

### 2b. Run Database Migrations

```bash
cd C:\Dev\repos\SimplePubManager

# Install EF Core tools if needed
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add Initial --project src/SimplePubManager.Infrastructure --startup-project src/SimplePubManager.Api

# Apply migration to database
dotnet ef database update --project src/SimplePubManager.Infrastructure --startup-project src/SimplePubManager.Api
```

**Expected Output:**
```
Done. To undo this action, use 'ef migrations remove'
Executed 1 migration (XX.XXXms)
```

## Step 3: Start Backend API

```bash
cd C:\Dev\repos\SimplePubManager

# Run the API
dotnet run --project src/SimplePubManager.Api

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5000
#       Application started. Press Ctrl+C to exit.
```

**API is now running at:** `http://localhost:5000`

## Step 4: Start Frontend

### 4a. Install Dependencies

```bash
cd C:\Dev\repos\SimplePubManager\client

npm install --legacy-peer-deps
```

### 4b. Configure Environment

Create `.env` file in `client/` directory:

```
REACT_APP_API_URL=http://localhost:5000/api/v1
REACT_APP_ORG_ID=00000000-0000-0000-0000-000000000001
```

### 4c. Start React App

```bash
npm start

# Expected output:
# Compiled successfully!
# 
# You can now view simplepubmanager in the browser.
# 
#   Local:            http://localhost:3000
#   On Your Network:  http://xxx.xxx.x.xxx:3000
```

**Frontend is now running at:** `http://localhost:3000`

## Step 5: Test Login

### Test Account (Pre-seeded in database after migrations)

**Manager Account:**
- Email: `manager@test.com`
- Password: `TestPass123!`
- Role: Manager
- Full access to all features

**Supervisor Account:**
- Email: `supervisor@test.com`
- Password: `TestPass123!`
- Role: Supervisor
- Can approve shifts and holidays

**Staff Account:**
- Email: `staff@test.com`
- Password: `TestPass123!`
- Role: Staff
- Can view own shifts, clock in/out, request holidays

## Step 6: Test Key Workflows

### 6a. Test Login
1. Open http://localhost:3000
2. Click "Login"
3. Enter manager@test.com / TestPass123!
4. Should redirect to Dashboard

### 6b. Test Ad-Hoc Shift Creation
1. Log in as staff@test.com
2. Should auto-create an ad-hoc shift on login
3. Click "Clock In" to start shift
4. Wait ~30 seconds
5. Click "Clock Out" to end shift
6. Shift status should be "Pending Approval"

### 6c. Test Shift Approval (Manager)
1. Log in as manager@test.com
2. Navigate to Shifts
3. Find the pending shift from Step 6b
4. Click "Approve Shift"
5. Should calculate payment (hours × rate)

### 6d. Test Task Assignment
1. As manager, go to Tasks
2. Create new task
3. Assign to "Kitchen" area or specific staff
4. Assign to staff who has shift in that area
5. Staff should see task in their Tasks list
6. Staff can mark complete with notes

### 6e. Test Holiday Request
1. Log in as staff@test.com
2. Navigate to Holidays
3. Click "Request Holiday"
4. Select date range, type (paid/unpaid)
5. Submit request
6. Log in as manager
7. Approve or reject the request

## Step 7: Run Integration Tests

```bash
cd C:\Dev\repos\SimplePubManager

dotnet test tests/SimplePubManager.Api.Tests

# Expected output:
# Test Run Successful.
# Total tests: 60
#   Passed: 60
#   Failed: 0
#   Skipped: 0
```

## Troubleshooting

### Issue: "Cannot connect to database"
- **Solution:** Verify PostgreSQL is running and credentials in appsettings.json are correct
- Check: `psql -U simplepubmanager -d simplepubmanager`

### Issue: "Migration failed"
- **Solution:** Drop and recreate database
  ```bash
  psql -U postgres
  DROP DATABASE simplepubmanager;
  CREATE DATABASE simplepubmanager;
  \q
  ```
- Re-run migrations

### Issue: "npm install fails"
- **Solution:** Use `--legacy-peer-deps` flag
  ```bash
  npm install --legacy-peer-deps
  ```

### Issue: "API returns 401 Unauthorized"
- **Solution:** Ensure JWT Secret is at least 32 characters in appsettings.json
- Check token is being sent in Authorization header

### Issue: "Frontend shows CORS error"
- **Solution:** Verify CORS is configured for http://localhost:3000 in appsettings.json
- Check: `"AllowedOrigins": ["http://localhost:3000", "http://localhost:5000"]`

## Verification Checklist

- [ ] PostgreSQL running on localhost:5432
- [ ] Database created: `simplepubmanager`
- [ ] User created: `simplepubmanager`
- [ ] Migrations applied successfully
- [ ] API starts on http://localhost:5000
- [ ] Frontend starts on http://localhost:3000
- [ ] Can login with manager@test.com
- [ ] Can create and approve shifts
- [ ] Can assign and complete tasks
- [ ] Can request and approve holidays
- [ ] All 60 integration tests pass

## Next Steps

1. **Explore Features** — Walk through each major workflow
2. **Test Error Cases** — Try invalid inputs, unauthorized access
3. **Monitor Logs** — Check API logs for errors
4. **Performance Testing** — Verify response times
5. **Deployment** — Set up Docker/cloud deployment

---

**Questions or Issues?** Check the API logs in terminal for detailed error messages.
