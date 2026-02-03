# How to Run TaxHelperToday Project

## Prerequisites

✅ **.NET 10.0 SDK** - Installed (version 10.0.102 detected)
- PostgreSQL 15+ must be installed and running
- Database `TaxHelperToday` must exist

## Step-by-Step Setup

### 1. Create PostgreSQL Database

Open PostgreSQL command line (psql) or pgAdmin and run:

```sql
CREATE DATABASE TaxHelperToday;
```

Or using command line:
```bash
psql -U postgres -c "CREATE DATABASE TaxHelperToday;"
```

### 2. Update Connection String

Edit `appsettings.json` and update the connection string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaxHelperToday;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

**Current setting**: Password is `P@ssw0rd` - update if your PostgreSQL password is different.

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

Or for development with hot reload:

```bash
dotnet watch run
```

## What Happens on First Run

When you run the application for the first time:

1. ✅ **Database tables** are created automatically
2. ✅ **Default roles** are seeded (SuperAdmin, Admin, Editor, Support)
3. ✅ **Admin user** is created automatically:
   - Email: `admin@taxhelpertoday.com`
   - Password: `Admin@123`
   - ⚠️ **Change this password after first login!**
4. ✅ **Content data** is seeded (Services, Blogs, FAQs, Pages) in Development mode

## Access the Application

Once running, the application will be available at:

- **Public Website**: 
  - HTTP: `http://localhost:5000`
  - HTTPS: `https://localhost:5001`

- **Admin Panel**: 
  - Login: `http://localhost:5000/Admin/Login`
  - Email: `admin@taxhelpertoday.com`
  - Password: `Admin@123`

## Troubleshooting

### Database Connection Error

If you see: `Error connecting to database`

1. Check PostgreSQL is running:
   ```bash
   # Windows (PowerShell)
   Get-Service postgresql*
   
   # Or check if port 5432 is listening
   netstat -an | findstr 5432
   ```

2. Verify connection string in `appsettings.json`
3. Test connection manually:
   ```bash
   psql -U postgres -d TaxHelperToday -c "SELECT version();"
   ```

### Port Already in Use

If port 5000/5001 is already in use:

1. Check what's using the port:
   ```bash
   netstat -ano | findstr :5000
   ```

2. Change the port in `Properties/launchSettings.json` or use:
   ```bash
   dotnet run --urls "http://localhost:5002"
   ```

### Missing Dependencies

If build fails:

```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

## Development Tips

### Watch Mode (Auto-reload on changes)

```bash
dotnet watch run
```

### Run with Specific Environment

```bash
# Development
dotnet run --environment Development

# Production
dotnet run --environment Production
```

### View Logs

Logs are written to:
- Console output
- File: `logs/log-YYYYMMDD.txt`

## Quick Start (All-in-One)

If everything is set up correctly:

```bash
# 1. Restore packages
dotnet restore

# 2. Build
dotnet build

# 3. Run
dotnet run
```

Then open: `http://localhost:5000`

---

## Default Admin Credentials

- **Email**: `admin@taxhelpertoday.com`
- **Password**: `Admin@123`

⚠️ **IMPORTANT**: Change the password immediately after first login!
