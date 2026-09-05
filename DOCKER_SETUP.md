# SimplePubManager - Docker Setup Guide

**Recommended Setup** - No PostgreSQL installation needed!

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- .NET 8 SDK
- Node.js 18+
- Git

## Step 1: Start PostgreSQL with Docker

### 1a. Verify Docker is Running

```bash
docker --version
# Should show: Docker version XX.X.X
```

### 1b. Start PostgreSQL Container

From the project root directory:

```bash
cd C:\Dev\repos\SimplePubManager

# Start PostgreSQL in the background
docker-compose up -d

# Expected output:
# Creating network "simplepubmanager_simplepubmanager-network" with driver "bridge"
# Creating simplepubmanager-db ... done
```

### 1c. Verify PostgreSQL is Running

```bash
# Check container status
docker-compose ps

# Expected output:
# NAME                  STATUS
# simplepubmanager-db   Up X seconds (healthy)
```

### 1d. Test Database Connection

```bash
# Connect to the database
docker-compose exec postgres psql -U simplepubmanager -d simplepubmanager

# You should see the psql prompt:
# simplepubmanager=#

# Type \q to exit
# \q
```

---

## Step 2: Configure Backend

### 2a. Verify Connection String

Check `src/SimplePubManager.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=simplepubmanager;Username=simplepubmanager;Password=simplepubmanager"
}
```

This is already correct! (Database created by Docker init script)

### 2b. Run Database Migrations

```bash
cd C:\Dev\repos\SimplePubManager

# Apply migrations
dotnet ef database update \
  --project src/SimplePubManager.Infrastructure \
  --startup-project src/SimplePubManager.Api

# Expected output:
# Executed 1 migration (XX.XXXms)
```

---

## Step 3: Start Backend API

```bash
# Terminal 1
cd C:\Dev\repos\SimplePubManager

dotnet run --project src/SimplePubManager.Api

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5000
#       Application started. Press Ctrl+C to exit.
```

---

## Step 4: Start Frontend

```bash
# Terminal 2
cd C:\Dev\repos\SimplePubManager\client

npm install --legacy-peer-deps
npm start

# Expected output:
# Compiled successfully!
# Local:            http://localhost:3000
```

---

## Step 5: Test Login

1. Open http://localhost:3000
2. Login with:
   - Email: `manager@test.com`
   - Password: `TestPass123!`
3. Should see Dashboard

---

## Docker Commands Reference

```bash
# Start PostgreSQL (background)
docker-compose up -d

# Stop PostgreSQL
docker-compose down

# Stop and remove data
docker-compose down -v

# View logs
docker-compose logs -f postgres

# Access database shell
docker-compose exec postgres psql -U simplepubmanager -d simplepubmanager

# Restart PostgreSQL
docker-compose restart

# View running containers
docker-compose ps

# Remove everything (clean slate)
docker-compose down -v && docker-compose up -d
```

---

## Verify Setup

Run the integration tests:

```bash
cd C:\Dev\repos\SimplePubManager

dotnet test tests/SimplePubManager.Api.Tests

# Expected: All 60 tests pass
```

---

## Troubleshooting

### Issue: Docker container not starting

```bash
# Check logs
docker-compose logs postgres

# Solution: Remove and recreate
docker-compose down -v
docker-compose up -d
```

### Issue: "Connection refused" when running migrations

```bash
# Wait for container to be healthy
docker-compose ps
# Should show: (healthy)

# If not healthy, wait 10 seconds and retry
sleep 10
dotnet ef database update ...
```

### Issue: "Database already exists"

```bash
# Clean up and start fresh
docker-compose down -v
docker-compose up -d
dotnet ef database update --project src/SimplePubManager.Infrastructure --startup-project src/SimplePubManager.Api
```

### Issue: Port 5432 already in use

```bash
# Option 1: Use different port in docker-compose.yml
# Change: "5432:5432" to "5433:5432"
# Then update appsettings.json: "Port=5433"

# Option 2: Kill process using port 5432
# Windows:
netstat -ano | findstr :5432
taskkill /PID <PID> /F

# Mac/Linux:
lsof -i :5432
kill -9 <PID>
```

---

## Quick Start Summary

```bash
# 1. Start PostgreSQL
docker-compose up -d

# 2. Apply migrations
dotnet ef database update --project src/SimplePubManager.Infrastructure --startup-project src/SimplePubManager.Api

# 3. Terminal 1: Start API
dotnet run --project src/SimplePubManager.Api

# 4. Terminal 2: Start Frontend
cd client && npm install --legacy-peer-deps && npm start

# 5. Open http://localhost:3000 and login!
```

---

## Database Access

### Via Docker CLI

```bash
docker-compose exec postgres psql -U simplepubmanager -d simplepubmanager

# Useful commands:
\dt           # List tables
\d <table>    # Describe table
SELECT * FROM public.users;  # Query data
\q            # Quit
```

### Via Database GUI (Optional)

Install DBeaver or pgAdmin:

```bash
# DBeaver connection:
Host: localhost
Port: 5432
Database: simplepubmanager
User: simplepubmanager
Password: simplepubmanager
```

---

## Cleanup

When done developing:

```bash
# Stop containers but keep data
docker-compose down

# Stop containers and delete all data
docker-compose down -v

# Remove everything including images
docker-compose down -v --rmi all
```

---

**That's it! Docker handles PostgreSQL completely. Happy coding! 🚀**
