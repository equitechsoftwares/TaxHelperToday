# TaxHelperToday - ASP.NET Core Migration Project

This project migrates the static TaxHelperToday website to a fully dynamic ASP.NET Core application using Razor Pages, PostgreSQL, AdminLTE, and JWT authentication with a modular monolith architecture.

---

## 📚 Documentation

### Main Documents

1. **[MIGRATION_PLAN.md](./MIGRATION_PLAN.md)** - Comprehensive migration plan
   - Architecture overview
   - Database schema design
   - Module breakdown
   - Implementation phases (10 weeks)
   - Security considerations
   - Testing strategy

2. **[QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)** - Get started quickly
   - Prerequisites
   - Step-by-step setup instructions
   - Database configuration
   - JWT setup
   - AdminLTE integration
   - Common issues & solutions

3. **[PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md)** - Complete folder structure
   - Modular monolith organization
   - Module responsibilities
   - Naming conventions
   - File organization tips

4. **[database-schema.sql](./database-schema.sql)** - Complete database schema
   - All tables with indexes
   - Foreign key relationships
   - Triggers for timestamps
   - Initial seed data

---

## 🎯 Project Overview

### Current State
- Static HTML pages
- JavaScript-based content (data.js)
- No backend database
- No admin interface

### Target State
- Dynamic ASP.NET Core Razor Pages
- PostgreSQL database
- AdminLTE admin panel
- JWT authentication
- Modular monolith architecture
- Full CRUD operations for content

---

## 🏗️ Architecture

### Modular Monolith Structure
```
TaxHelperToday/
├── TaxHelperToday.Web/              # Main web app (Razor Pages)
├── TaxHelperToday.Shared/           # Shared DTOs, contracts
├── TaxHelperToday.Infrastructure/   # Database, external services
└── Modules/
    ├── Identity/                    # Authentication & authorization
    ├── Content/                     # Blogs, Services, FAQs, Pages
    ├── Contact/                     # Contact forms & enquiries
    ├── Admin/                       # Admin panel (AdminLTE)
    └── Public/                      # Public view components
```

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages + AdminLTE 3.x
- **Database**: PostgreSQL 15+
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **Logging**: Serilog

---

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Setup Steps

1. **Clone/Create Project**
   ```bash
   dotnet new sln -n TaxHelperToday
   ```

2. **Set Up Database**
   - Create PostgreSQL database
   - Run `database-schema.sql`

3. **Configure Connection**
   - Update `appsettings.json` with database connection string

4. **Install Packages**
   ```bash
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```

5. **Run Application**
   ```bash
   dotnet run
   ```

For detailed instructions, see [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md).

---

## 📊 Database Schema

### Core Tables
- **Identity**: `users`, `roles`, `user_roles`, `refresh_tokens`
- **Content**: `blog_posts`, `blog_tags`, `services`, `faqs`, `pages`
- **Contact**: `contact_enquiries`, `mini_enquiries`
- **Admin**: `admin_settings`, `activity_logs`

See [database-schema.sql](./database-schema.sql) for complete schema.

---

## 🔐 Authentication

### JWT Configuration
- Access Token: 15 minutes expiry
- Refresh Token: 7 days expiry
- Algorithm: HS256
- Claims: UserId, Email, Roles

### Admin Login
- Route: `/admin/login`
- JWT-based authentication
- Role-based authorization (SuperAdmin, Admin, Editor, Support)

---

## 📦 Modules

### 1. Identity Module
- User registration/login
- JWT token management
- Role-based access control
- Password hashing (BCrypt)

### 2. Content Module
- Blog post CRUD
- Service management
- FAQ management
- Static page management

### 3. Contact Module
- Contact form processing
- Mini enquiry handling
- Enquiry assignment workflow

### 4. Admin Module
- AdminLTE dashboard
- Content management UI
- User management
- Activity logging

### 5. Public Module
- Public Razor Pages
- View components (Header, Footer)
- Dynamic content rendering

---

## 📅 Implementation Timeline

| Phase | Duration | Focus |
|-------|----------|-------|
| Phase 1 | 2 weeks | Foundation & Database |
| Phase 2 | 1 week | Identity & JWT |
| Phase 3 | 1 week | Admin Panel Setup |
| Phase 4 | 1 week | Content Backend |
| Phase 5 | 1 week | Content Admin UI |
| Phase 6 | 1 week | Public Razor Pages |
| Phase 7 | 1 week | Contact Module |
| Phase 8 | 1 week | Testing |
| Phase 9 | 1 week | Deployment |
| **Total** | **10 weeks** | **Production Ready** |

---

## 🎨 Admin Panel Features

### AdminLTE Integration
- Modern, responsive admin interface
- Sidebar navigation
- Data tables for listings
- Form modals
- Toast notifications

### Admin Routes
```
/admin/login              # Login page
/admin/dashboard          # Dashboard
/admin/content/blogs      # Blog management
/admin/content/services   # Service management
/admin/content/faqs       # FAQ management
/admin/contact/enquiries  # Contact enquiries
/admin/users              # User management
```

---

## 📝 Migration Strategy

### Data Migration
1. Extract data from `data.js`
2. Parse JSON structures
3. Insert into PostgreSQL tables
4. Generate SEO-friendly slugs
5. Set published dates

### Page Migration
1. Convert HTML to Razor Pages
2. Replace static content with database queries
3. Implement view components for reusable UI
4. Add dynamic navigation
5. Preserve existing styling

---

## 🔒 Security Features

- **Password Security**: BCrypt hashing (12+ rounds)
- **JWT Security**: HttpOnly cookies, token rotation
- **API Security**: Rate limiting, input validation
- **Data Protection**: HTTPS only, secure headers
- **SQL Injection**: EF Core parameterized queries
- **XSS Prevention**: Razor auto-encoding

---

## 🧪 Testing Strategy

- **Unit Tests**: Service layer, domain logic
- **Integration Tests**: API endpoints, database operations
- **E2E Tests**: Critical user journeys
- **Target Coverage**: > 70%

---

## 📈 Performance Optimization

- Database indexes on frequently queried columns
- Connection pooling
- Caching for static content (future: Redis)
- Minified CSS/JS
- Image optimization
- CDN for static assets

---

## 🛠️ Development Workflow

### Recommended Tools
- **IDE**: Visual Studio 2022 or VS Code
- **Database**: pgAdmin or DBeaver
- **API Testing**: Postman or Thunder Client
- **Version Control**: Git

### Code Quality
- Follow C# coding conventions
- Use dependency injection
- Implement repository pattern
- Write unit tests
- Document complex logic

---

## 📞 Support

### Getting Help
1. Check [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md) for common issues
2. Review [MIGRATION_PLAN.md](./MIGRATION_PLAN.md) for architecture details
3. Refer to [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md) for organization

### Resources
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [AdminLTE Documentation](https://adminlte.io/docs/3.2/)

---

## ✅ Success Criteria

- [x] Migration plan created
- [x] Database schema designed
- [x] Project structure defined
- [ ] All static pages converted to dynamic
- [ ] Admin can manage all content
- [ ] JWT authentication working
- [ ] All forms functional
- [ ] Performance targets met
- [ ] Security audit passed
- [ ] Test coverage > 70%

---

## 📄 License

This project is proprietary software for TaxHelperToday.

---

## 🎯 Next Steps

1. **Review Documentation**
   - Read MIGRATION_PLAN.md thoroughly
   - Understand architecture decisions
   - Review database schema

2. **Set Up Environment**
   - Install prerequisites
   - Create database
   - Configure connection strings

3. **Start Development**
   - Follow QUICK_START_GUIDE.md
   - Begin with Phase 1 (Foundation)
   - Implement modules incrementally

4. **Track Progress**
   - Use project management tool
   - Update status regularly
   - Document decisions

---

**Ready to start?** Begin with [QUICK_START_GUIDE.md](./QUICK_START_GUIDE.md)!

---

*Last Updated: 2024*  
*Version: 1.0*
