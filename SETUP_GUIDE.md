# TaxHelperToday - Setup Guide

This guide will help you set up the TaxHelperToday application for development.

## Prerequisites

1. **.NET 10.0 SDK** - Download from [Microsoft](https://dotnet.microsoft.com/download)
2. **PostgreSQL 15+** - Download from [PostgreSQL](https://www.postgresql.org/download/)
3. **Visual Studio 2022** or **VS Code** with C# extension
4. **Git** (optional, for version control)

## Database Setup

### 1. Install PostgreSQL

Install PostgreSQL 15 or later on your system. During installation, remember the password you set for the `postgres` user.

### 2. Create Database

Open PostgreSQL command line (psql) or pgAdmin and run:

```sql
CREATE DATABASE TaxHelperToday;
```

### 3. Run Database Schema

Execute the SQL script located at `Html/database-schema.sql`:

```bash
psql -U postgres -d TaxHelperToday -f Html/database-schema.sql
```

Or using pgAdmin:
1. Connect to your PostgreSQL server
2. Right-click on `TaxHelperToday` database
3. Select "Query Tool"
4. Open and execute `Html/database-schema.sql`

### 4. Update Connection String

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaxHelperToday;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Replace `YOUR_PASSWORD` with your PostgreSQL password.

## Application Setup

### 1. Restore NuGet Packages

```bash
dotnet restore
```

### 2. Build the Application

```bash
dotnet build
```

### 3. Run the Application

```bash
dotnet run
```

The application will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## Default Admin Credentials

After running the database schema, a default admin user is created:

- **Email**: `admin@taxhelpertoday.com`
- **Password**: `Admin@123` (⚠️ **CHANGE THIS IMMEDIATELY AFTER FIRST LOGIN**)

The password hash in the database schema is a placeholder. You'll need to create the admin user properly using the registration endpoint or seed data.

## JWT Configuration

The JWT settings are configured in `appsettings.json`. For production, **MUST** change the `SecretKey` to a secure random string (at least 32 characters).

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32CharactersLong!",
    "Issuer": "TaxHelperToday",
    "Audience": "TaxHelperToday",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

## Project Structure

```
TaxHelperToday/
├── TaxHelperToday.Shared/          # Shared constants, DTOs
├── TaxHelperToday.Infrastructure/  # Database, EF Core configurations
├── TaxHelperToday.Modules/
│   ├── Identity/                   # Authentication & authorization
│   ├── Content/                    # Blog, Services, FAQs, Pages
│   ├── Contact/                    # Contact forms & enquiries
│   ├── Admin/                      # Admin panel functionality
│   └── Public/                     # Public-facing Razor Pages
└── Pages/                          # Razor Pages
```

## Development Workflow

### Running Migrations

The application uses `EnsureCreated()` for development. For production, use EF Core migrations:

```bash
# Create a migration
dotnet ef migrations add InitialCreate --project TaxHelperToday.csproj

# Apply migrations
dotnet ef database update --project TaxHelperToday.csproj
```

### Logging

Logs are written to:
- Console (during development)
- `logs/log-YYYYMMDD.txt` files

## Next Steps

1. ✅ Database setup complete
2. ✅ JWT authentication configured
3. ⏭️ Create admin login page (Phase 3)
4. ⏭️ Implement content management (Phase 4-5)
5. ⏭️ Convert static HTML to Razor Pages (Phase 6)

## Troubleshooting

### Database Connection Issues

- Verify PostgreSQL is running: `pg_isready`
- Check connection string in `appsettings.json`
- Ensure database `TaxHelperToday` exists
- Verify username and password are correct

### Build Errors

- Run `dotnet clean` and `dotnet restore`
- Ensure all NuGet packages are installed
- Check that .NET 10.0 SDK is installed

### JWT Token Issues

- Verify `JwtSettings:SecretKey` is set in `appsettings.json`
- Ensure the secret key is at least 32 characters long
- Check token expiration settings

## Support

For issues or questions, refer to the `MIGRATION_PLAN.md` document for detailed architecture and implementation guidelines.
