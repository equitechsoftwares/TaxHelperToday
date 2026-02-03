# TaxHelperToday - Project Structure Reference

This document shows the complete folder structure for the modular monolith architecture.

```
TaxHelperToday/
│
├── .gitignore
├── README.md
├── MIGRATION_PLAN.md
├── QUICK_START_GUIDE.md
├── database-schema.sql
│
├── src/
│   │
│   ├── TaxHelperToday.Web/                    # Main Web Application (Razor Pages)
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── TaxHelperToday.Web.csproj
│   │   │
│   │   ├── Pages/                             # Public Razor Pages
│   │   │   ├── _ViewImports.cshtml
│   │   │   ├── _ViewStart.cshtml
│   │   │   ├── Shared/
│   │   │   │   ├── _Layout.cshtml
│   │   │   │   ├── _Header.cshtml
│   │   │   │   ├── _Footer.cshtml
│   │   │   │   └── _Navigation.cshtml
│   │   │   ├── Index.cshtml                   # Homepage
│   │   │   ├── About.cshtml
│   │   │   ├── Blogs/
│   │   │   │   ├── Index.cshtml               # Blog listing
│   │   │   │   └── Details.cshtml            # Blog detail
│   │   │   ├── Services/
│   │   │   │   ├── Index.cshtml              # Service listing
│   │   │   │   └── Details.cshtml           # Service detail
│   │   │   ├── FAQs.cshtml
│   │   │   ├── Contact.cshtml
│   │   │   ├── PrivacyPolicy.cshtml
│   │   │   ├── TermsConditions.cshtml
│   │   │   └── TrustSafety.cshtml
│   │   │
│   │   ├── wwwroot/                           # Static assets
│   │   │   ├── css/
│   │   │   │   └── site.css                   # Custom styles
│   │   │   ├── js/
│   │   │   │   └── site.js                    # Custom scripts
│   │   │   ├── img/
│   │   │   │   └── logo.png
│   │   │   └── lib/                           # Third-party libraries
│   │   │
│   │   └── Properties/
│   │       └── launchSettings.json
│   │
│   ├── TaxHelperToday.Shared/                 # Shared Contracts & DTOs
│   │   ├── TaxHelperToday.Shared.csproj
│   │   │
│   │   ├── Contracts/                         # Interfaces
│   │   │   ├── IEntity.cs
│   │   │   └── IAuditable.cs
│   │   │
│   │   ├── DTOs/                              # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   ├── LoginResponse.cs
│   │   │   │   └── RefreshTokenRequest.cs
│   │   │   ├── Blog/
│   │   │   │   ├── BlogPostDto.cs
│   │   │   │   └── CreateBlogPostDto.cs
│   │   │   ├── Service/
│   │   │   │   ├── ServiceDto.cs
│   │   │   │   └── CreateServiceDto.cs
│   │   │   └── Contact/
│   │   │       └── ContactEnquiryDto.cs
│   │   │
│   │   ├── Constants/                         # Application constants
│   │   │   ├── Roles.cs
│   │   │   ├── Permissions.cs
│   │   │   └── StatusCodes.cs
│   │   │
│   │   └── Exceptions/                        # Custom exceptions
│   │       ├── NotFoundException.cs
│   │       └── ValidationException.cs
│   │
│   ├── TaxHelperToday.Infrastructure/        # Infrastructure Layer
│   │   ├── TaxHelperToday.Infrastructure.csproj
│   │   │
│   │   ├── Data/                              # Database context
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/                # EF Core configurations
│   │   │       ├── UserConfiguration.cs
│   │   │       ├── BlogPostConfiguration.cs
│   │   │       └── ServiceConfiguration.cs
│   │   │
│   │   ├── Identity/                          # Identity services
│   │   │   ├── JwtTokenService.cs
│   │   │   ├── PasswordHasher.cs
│   │   │   └── TokenGenerator.cs
│   │   │
│   │   ├── Repositories/                      # Generic repository pattern
│   │   │   ├── IRepository.cs
│   │   │   └── Repository.cs
│   │   │
│   │   └── Services/                          # Infrastructure services
│   │       ├── EmailService.cs
│   │       └── FileStorageService.cs
│   │
│   └── Modules/                               # Feature Modules
│       │
│       ├── TaxHelperToday.Modules.Identity/
│       │   │
│       │   ├── TaxHelperToday.Modules.Identity.Domain/
│       │   │   ├── TaxHelperToday.Modules.Identity.Domain.csproj
│       │   │   │
│       │   │   ├── Entities/
│       │   │   │   ├── User.cs
│       │   │   │   ├── Role.cs
│       │   │   │   ├── UserRole.cs
│       │   │   │   └── RefreshToken.cs
│       │   │   │
│       │   │   └── Enums/
│       │   │       └── UserStatus.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Identity.Application/
│       │   │   ├── TaxHelperToday.Modules.Identity.Application.csproj
│       │   │   │
│       │   │   ├── Services/
│       │   │   │   ├── IIdentityService.cs
│       │   │   │   └── IdentityService.cs
│       │   │   │
│       │   │   ├── Validators/
│       │   │   │   └── LoginRequestValidator.cs
│       │   │   │
│       │   │   └── Mappings/
│       │   │       └── IdentityMappingProfile.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Identity.Infrastructure/
│       │   │   ├── TaxHelperToday.Modules.Identity.Infrastructure.csproj
│       │   │   │
│       │   │   ├── Repositories/
│       │   │   │   ├── IUserRepository.cs
│       │   │   │   └── UserRepository.cs
│       │   │   │
│       │   │   └── Persistence/
│       │   │       └── IdentityDbContext.cs
│       │   │
│       │   └── TaxHelperToday.Modules.Identity.Presentation/
│       │       ├── TaxHelperToday.Modules.Identity.Presentation.csproj
│       │       │
│       │       ├── Controllers/
│       │       │   └── AuthController.cs
│       │       │
│       │       └── Pages/
│       │           └── Admin/
│       │               └── Login.cshtml
│       │
│       ├── TaxHelperToday.Modules.Content/
│       │   │
│       │   ├── TaxHelperToday.Modules.Content.Domain/
│       │   │   ├── TaxHelperToday.Modules.Content.Domain.csproj
│       │   │   │
│       │   │   ├── Entities/
│       │   │   │   ├── BlogPost.cs
│       │   │   │   ├── BlogTag.cs
│       │   │   │   ├── BlogPostTag.cs
│       │   │   │   ├── Service.cs
│       │   │   │   ├── FAQ.cs
│       │   │   │   └── Page.cs
│       │   │   │
│       │   │   └── Enums/
│       │   │       ├── BlogPostStatus.cs
│       │   │       └── ServiceType.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Content.Application/
│       │   │   ├── TaxHelperToday.Modules.Content.Application.csproj
│       │   │   │
│       │   │   ├── Services/
│       │   │   │   ├── IBlogService.cs
│       │   │   │   ├── BlogService.cs
│       │   │   │   ├── IServiceService.cs
│       │   │   │   ├── ServiceService.cs
│       │   │   │   ├── IFaqService.cs
│       │   │   │   ├── FaqService.cs
│       │   │   │   ├── IPageService.cs
│       │   │   │   └── PageService.cs
│       │   │   │
│       │   │   └── Validators/
│       │   │       ├── CreateBlogPostValidator.cs
│       │   │       └── CreateServiceValidator.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Content.Infrastructure/
│       │   │   ├── TaxHelperToday.Modules.Content.Infrastructure.csproj
│       │   │   │
│       │   │   └── Repositories/
│       │   │       ├── IBlogPostRepository.cs
│       │   │       ├── BlogPostRepository.cs
│       │   │       ├── IServiceRepository.cs
│       │   │       └── ServiceRepository.cs
│       │   │
│       │   └── TaxHelperToday.Modules.Content.Presentation/
│       │       ├── TaxHelperToday.Modules.Content.Presentation.csproj
│       │       │
│       │       └── Controllers/
│       │           ├── BlogController.cs
│       │           ├── ServiceController.cs
│       │           └── FaqController.cs
│       │
│       ├── TaxHelperToday.Modules.Contact/
│       │   │
│       │   ├── TaxHelperToday.Modules.Contact.Domain/
│       │   │   ├── TaxHelperToday.Modules.Contact.Domain.csproj
│       │   │   │
│       │   │   └── Entities/
│       │   │       ├── ContactEnquiry.cs
│       │   │       └── MiniEnquiry.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Contact.Application/
│       │   │   ├── TaxHelperToday.Modules.Contact.Application.csproj
│       │   │   │
│       │   │   └── Services/
│       │   │       ├── IContactService.cs
│       │   │       └── ContactService.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Contact.Infrastructure/
│       │   │   ├── TaxHelperToday.Modules.Contact.Infrastructure.csproj
│       │   │   │
│       │   │   └── Repositories/
│       │   │       └── ContactRepository.cs
│       │   │
│       │   └── TaxHelperToday.Modules.Contact.Presentation/
│       │       ├── TaxHelperToday.Modules.Contact.Presentation.csproj
│       │       │
│       │       └── Controllers/
│       │           └── ContactController.cs
│       │
│       ├── TaxHelperToday.Modules.Admin/
│       │   │
│       │   ├── TaxHelperToday.Modules.Admin.Domain/
│       │   │   ├── TaxHelperToday.Modules.Admin.Domain.csproj
│       │   │   │
│       │   │   └── Entities/
│       │   │       ├── AdminSetting.cs
│       │   │       └── ActivityLog.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Admin.Application/
│       │   │   ├── TaxHelperToday.Modules.Admin.Application.csproj
│       │   │   │
│       │   │   └── Services/
│       │   │       ├── IAdminService.cs
│       │   │       ├── AdminService.cs
│       │   │       ├── IUserManagementService.cs
│       │   │       └── UserManagementService.cs
│       │   │
│       │   ├── TaxHelperToday.Modules.Admin.Infrastructure/
│       │   │   ├── TaxHelperToday.Modules.Admin.Infrastructure.csproj
│       │   │   │
│       │   │   └── Repositories/
│       │   │       └── AdminRepository.cs
│       │   │
│       │   └── TaxHelperToday.Modules.Admin.Presentation/
│       │       ├── TaxHelperToday.Modules.Admin.Presentation.csproj
│       │       │
│       │       ├── Pages/
│       │       │   └── Admin/
│       │       │       ├── _AdminLayout.cshtml
│       │       │       ├── Dashboard.cshtml
│       │       │       ├── Content/
│       │       │       │   ├── Blogs/
│       │       │       │   │   ├── Index.cshtml
│       │       │       │   │   ├── Create.cshtml
│       │       │       │   │   └── Edit.cshtml
│       │       │       │   ├── Services/
│       │       │       │   │   ├── Index.cshtml
│       │       │       │   │   ├── Create.cshtml
│       │       │       │   │   └── Edit.cshtml
│       │       │       │   ├── FAQs/
│       │       │       │   │   ├── Index.cshtml
│       │       │       │   │   ├── Create.cshtml
│       │       │       │   │   └── Edit.cshtml
│       │       │       │   └── Pages/
│       │       │       │       └── Edit.cshtml
│       │       │       ├── Contact/
│       │       │       │   ├── Enquiries/
│       │       │       │   │   └── Index.cshtml
│       │       │       │   └── MiniEnquiries/
│       │       │       │       └── Index.cshtml
│       │       │       └── Users/
│       │       │           ├── Index.cshtml
│       │       │           └── Roles.cshtml
│       │       │
│       │       └── Controllers/
│       │           └── AdminController.cs
│       │
│       └── TaxHelperToday.Modules.Public/
│           ├── TaxHelperToday.Modules.Public.csproj
│           │
│           └── ViewComponents/
│               ├── HeaderViewComponent.cs
│               ├── FooterViewComponent.cs
│               └── NavigationViewComponent.cs
│
└── tests/                                    # Test Projects
    ├── TaxHelperToday.Modules.Identity.Tests/
    │   ├── TaxHelperToday.Modules.Identity.Tests.csproj
    │   └── Services/
    │       └── IdentityServiceTests.cs
    │
    ├── TaxHelperToday.Modules.Content.Tests/
    │   ├── TaxHelperToday.Modules.Content.Tests.csproj
    │   └── Services/
    │       └── BlogServiceTests.cs
    │
    └── TaxHelperToday.Infrastructure.Tests/
        ├── TaxHelperToday.Infrastructure.Tests.csproj
        └── Repositories/
            └── RepositoryTests.cs
```

---

## Key Structure Principles

### 1. **Modular Monolith**
- Each module is self-contained with Domain, Application, Infrastructure, and Presentation layers
- Modules communicate through shared contracts (interfaces)
- Clear separation of concerns

### 2. **Domain-Driven Design (DDD)**
- Domain layer contains entities and business logic
- Application layer contains use cases and services
- Infrastructure layer handles data access and external services
- Presentation layer handles UI/API

### 3. **Dependency Direction**
```
Presentation → Application → Domain
Infrastructure → Domain
```

### 4. **Shared Code**
- Common DTOs, interfaces, and constants in `TaxHelperToday.Shared`
- Infrastructure services in `TaxHelperToday.Infrastructure`
- Database context can be shared or module-specific

---

## Module Responsibilities

### Identity Module
- User authentication and authorization
- JWT token management
- Role-based access control
- User management

### Content Module
- Blog post management
- Service management
- FAQ management
- Static page management

### Contact Module
- Contact form processing
- Enquiry management
- Email notifications

### Admin Module
- Admin dashboard
- System settings
- Activity logging
- User management UI

### Public Module
- View components for shared UI
- Public-facing pages (can be in main Web project)

---

## Naming Conventions

- **Entities**: PascalCase (e.g., `BlogPost`, `User`)
- **Services**: PascalCase with "Service" suffix (e.g., `BlogService`)
- **Repositories**: PascalCase with "Repository" suffix (e.g., `BlogPostRepository`)
- **DTOs**: PascalCase with "Dto" suffix (e.g., `BlogPostDto`)
- **Controllers**: PascalCase with "Controller" suffix (e.g., `BlogController`)
- **Pages**: PascalCase (e.g., `Index.cshtml`, `Create.cshtml`)

---

## File Organization Tips

1. **Keep related files together** - Group by feature, not by type
2. **Use folders for organization** - Don't put everything in root
3. **Follow conventions** - Stick to ASP.NET Core conventions
4. **Separate concerns** - Domain, Application, Infrastructure, Presentation
5. **Shared code in Shared project** - Avoid duplication

---

This structure provides a solid foundation for a scalable, maintainable modular monolith application.
