# Quick Start Guide - TaxHelperToday Migration

This guide will help you get started with the migration project quickly.

## Prerequisites

### Required Software
1. **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **PostgreSQL 15+** - [Download](https://www.postgresql.org/download/)
3. **Visual Studio 2022** or **VS Code** with C# extension
4. **Git** - For version control
5. **Node.js 18+** (optional) - For AdminLTE assets

### Development Tools
- **pgAdmin** or **DBeaver** - PostgreSQL management
- **Postman** or **Thunder Client** - API testing

---

## Step 1: Database Setup

### 1.1 Install PostgreSQL
- Download and install PostgreSQL 15 or higher
- Note your `postgres` user password during installation

### 1.2 Create Database
```sql
-- Connect to PostgreSQL as postgres user
CREATE DATABASE taxhelpertoday;
CREATE USER taxhelper_user WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE taxhelpertoday TO taxhelper_user;
```

### 1.3 Run Schema Script
```bash
# Using psql command line
psql -U taxhelper_user -d taxhelpertoday -f database-schema.sql

# Or using pgAdmin
# Open pgAdmin → Connect to server → Right-click on database → Query Tool → Paste and run database-schema.sql
```

### 1.4 Verify Database
```sql
-- Check if tables were created
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;
```

---

## Step 2: Create ASP.NET Core Solution

### 2.1 Create Solution Structure
```bash
# Create solution
dotnet new sln -n TaxHelperToday

# Create main web project
dotnet new web -n TaxHelperToday.Web -o src/TaxHelperToday.Web

# Create shared project
dotnet new classlib -n TaxHelperToday.Shared -o src/TaxHelperToday.Shared

# Create infrastructure project
dotnet new classlib -n TaxHelperToday.Infrastructure -o src/TaxHelperToday.Infrastructure

# Create module projects (example for Identity module)
dotnet new classlib -n TaxHelperToday.Modules.Identity.Domain -o src/TaxHelperToday.Modules.Identity/TaxHelperToday.Modules.Identity.Domain
dotnet new classlib -n TaxHelperToday.Modules.Identity.Application -o src/TaxHelperToday.Modules.Identity/TaxHelperToday.Modules.Identity.Application
dotnet new classlib -n TaxHelperToday.Modules.Identity.Infrastructure -o src/TaxHelperToday.Modules.Identity/TaxHelperToday.Modules.Identity.Infrastructure
dotnet new web -n TaxHelperToday.Modules.Identity.Presentation -o src/TaxHelperToday.Modules.Identity/TaxHelperToday.Modules.Identity.Presentation

# Add projects to solution
dotnet sln add src/TaxHelperToday.Web/TaxHelperToday.Web.csproj
dotnet sln add src/TaxHelperToday.Shared/TaxHelperToday.Shared.csproj
dotnet sln add src/TaxHelperToday.Infrastructure/TaxHelperToday.Infrastructure.csproj
# ... add other projects
```

### 2.2 Install Required NuGet Packages

**For Web Project:**
```bash
cd src/TaxHelperToday.Web
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Serilog.AspNetCore
dotnet add package FluentValidation.AspNetCore
```

**For Infrastructure Project:**
```bash
cd src/TaxHelperToday.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package BCrypt.Net-Next
```

---

## Step 3: Configure Connection String

### 3.1 Create appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taxhelpertoday;Username=taxhelper_user;Password=your_secure_password"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-characters-long-for-production",
    "Issuer": "TaxHelperToday",
    "Audience": "TaxHelperToday",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3.2 Create appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taxhelpertoday_dev;Username=taxhelper_user;Password=your_secure_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

---

## Step 4: Set Up Entity Framework Core

### 4.1 Create DbContext
```csharp
// src/TaxHelperToday.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace TaxHelperToday.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSets here as you create entities
    // public DbSet<User> Users { get; set; }
    // public DbSet<BlogPost> BlogPosts { get; set; }
    // etc.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add entity configurations here
    }
}
```

### 4.2 Configure DbContext in Program.cs
```csharp
// src/TaxHelperToday.Web/Program.cs
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// ... rest of configuration
```

### 4.3 Create Initial Migration
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate --project src/TaxHelperToday.Infrastructure --startup-project src/TaxHelperToday.Web

# Apply migration (or use database-schema.sql directly)
dotnet ef database update --project src/TaxHelperToday.Infrastructure --startup-project src/TaxHelperToday.Web
```

---

## Step 5: Set Up JWT Authentication

### 5.1 Configure JWT in Program.cs
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
```

---

## Step 6: Set Up AdminLTE

### 6.1 Install AdminLTE
```bash
# Option 1: Using npm (if you have Node.js)
cd src/TaxHelperToday.Web
npm init -y
npm install admin-lte@^3.2

# Option 2: Download from CDN (simpler for start)
# Add to _Layout.cshtml
```

### 6.2 Create Admin Layout
```html
<!-- Views/Shared/_AdminLayout.cshtml -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Admin - TaxHelperToday</title>
    
    <!-- AdminLTE CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/admin-lte@3.2/dist/css/adminlte.min.css">
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
</head>
<body class="hold-transition sidebar-mini">
    <div class="wrapper">
        <!-- Navbar -->
        <nav class="main-header navbar navbar-expand navbar-white navbar-light">
            <!-- Left navbar links -->
            <ul class="navbar-nav">
                <li class="nav-item">
                    <a class="nav-link" data-widget="pushmenu" href="#" role="button">
                        <i class="fas fa-bars"></i>
                    </a>
                </li>
            </ul>
            
            <!-- Right navbar links -->
            <ul class="navbar-nav ml-auto">
                <li class="nav-item">
                    <a class="nav-link" href="/admin/logout">
                        <i class="fas fa-sign-out-alt"></i> Logout
                    </a>
                </li>
            </ul>
        </nav>

        <!-- Main Sidebar -->
        <aside class="main-sidebar sidebar-dark-primary elevation-4">
            <a href="/admin/dashboard" class="brand-link">
                <span class="brand-text font-weight-light">TaxHelperToday</span>
            </a>

            <!-- Sidebar -->
            <div class="sidebar">
                <nav class="mt-2">
                    <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu">
                        <li class="nav-item">
                            <a href="/admin/dashboard" class="nav-link">
                                <i class="nav-icon fas fa-tachometer-alt"></i>
                                <p>Dashboard</p>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/admin/content/blogs" class="nav-link">
                                <i class="nav-icon fas fa-blog"></i>
                                <p>Blogs</p>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/admin/content/services" class="nav-link">
                                <i class="nav-icon fas fa-briefcase"></i>
                                <p>Services</p>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/admin/content/faqs" class="nav-link">
                                <i class="nav-icon fas fa-question-circle"></i>
                                <p>FAQs</p>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/admin/contact/enquiries" class="nav-link">
                                <i class="nav-icon fas fa-envelope"></i>
                                <p>Enquiries</p>
                            </a>
                        </li>
                    </ul>
                </nav>
            </div>
        </aside>

        <!-- Content Wrapper -->
        <div class="content-wrapper">
            @RenderBody()
        </div>

        <!-- Footer -->
        <footer class="main-footer">
            <strong>Copyright &copy; @DateTime.Now.Year TaxHelperToday.</strong>
            All rights reserved.
        </footer>
    </div>

    <!-- AdminLTE JS -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/admin-lte@3.2/dist/js/adminlte.min.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

## Step 7: Create First Admin User

### 7.1 Create Seed Data Script
```csharp
// Create a console app or use EF Core seeding
// Or manually insert via SQL:

-- Use the default admin from database-schema.sql
-- Or create a new one:

-- Generate password hash using BCrypt (use online tool or C# code)
-- Example hash for "Admin@123": $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5

INSERT INTO users (email, password_hash, full_name, is_active, is_email_verified)
VALUES ('admin@taxhelpertoday.com', '$2a$12$YOUR_BCRYPT_HASH_HERE', 'Admin User', true, true);

-- Assign SuperAdmin role
INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id
FROM users u, roles r
WHERE u.email = 'admin@taxhelpertoday.com' AND r.name = 'SuperAdmin';
```

---

## Step 8: Run the Application

### 8.1 Build and Run
```bash
cd src/TaxHelperToday.Web
dotnet build
dotnet run
```

### 8.2 Access Application
- **Public Site**: http://localhost:5000
- **Admin Login**: http://localhost:5000/admin/login

---

## Step 9: Migrate Static Content

### 9.1 Create Data Migration Script
Create a console application or use EF Core seeding to migrate data from `data.js`:

```csharp
// Example: Migrate Blog Posts
var blogPosts = new[]
{
    new BlogPost
    {
        Slug = "prepare-for-itr-season",
        Title = "How to prepare for ITR season without last-minute stress",
        Category = "Individuals",
        Excerpt = "A simple checklist to gather salary slips...",
        Content = "<p>Full content here...</p>",
        ReadTime = "6 min read",
        IsPublished = true,
        PublishedAt = DateTime.UtcNow
    },
    // ... more posts
};

context.BlogPosts.AddRange(blogPosts);
context.SaveChanges();
```

---

## Common Issues & Solutions

### Issue: Database Connection Failed
**Solution**: 
- Check PostgreSQL is running
- Verify connection string in appsettings.json
- Check firewall settings

### Issue: JWT Token Not Working
**Solution**:
- Ensure SecretKey is at least 32 characters
- Check token expiration settings
- Verify JWT middleware is configured correctly

### Issue: AdminLTE Not Loading
**Solution**:
- Check CDN links are correct
- Verify internet connection (if using CDN)
- Check browser console for errors

---

## Next Steps

1. **Implement Identity Module** - User authentication
2. **Create Admin Login Page** - JWT-based login
3. **Build Content Management** - CRUD for blogs, services, FAQs
4. **Convert Static Pages** - Migrate HTML to Razor Pages
5. **Implement Contact Forms** - Backend processing

---

## Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [AdminLTE Documentation](https://adminlte.io/docs/3.2/)
- [JWT Authentication](https://jwt.io/)

---

**Need Help?** Refer to the main MIGRATION_PLAN.md for detailed architecture and implementation guidance.
