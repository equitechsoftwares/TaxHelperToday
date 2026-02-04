using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Infrastructure.Middleware;
using TaxHelperToday.Infrastructure.Services;
using TaxHelperToday.Modules.Admin.Domain.Entities;
using TaxHelperToday.Modules.Content.Application.Services;
using TaxHelperToday.Modules.Content.Infrastructure.Services;
using TaxHelperToday.Modules.Identity.Application.Services;
using TaxHelperToday.Modules.Identity.Domain.Entities;
using TaxHelperToday.Modules.Identity.Infrastructure.Services;
using TaxHelperToday.Shared.Constants;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting TaxHelperToday application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddRazorPages();
    builder.Services.AddControllers();

    // Database Configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        }));

    // JWT Authentication Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
    var issuer = jwtSettings["Issuer"] ?? "TaxHelperToday";
    var audience = jwtSettings["Audience"] ?? "TaxHelperToday";

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
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        // Admin policy - requires any admin role
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole(Roles.SuperAdmin) ||
                context.User.IsInRole(Roles.Admin) ||
                context.User.IsInRole(Roles.Editor) ||
                context.User.IsInRole(Roles.Support)));
    });

    // Dependency Injection
    builder.Services.AddScoped<JwtTokenService>();
    builder.Services.AddScoped<PasswordHasher>();
    builder.Services.AddScoped<IIdentityService, IdentityService>();

    // Content Module Services
    builder.Services.AddScoped<IBlogService, BlogService>();
    builder.Services.AddScoped<IServiceService, ServiceService>();
    builder.Services.AddScoped<IFaqService, FaqService>();
    builder.Services.AddScoped<IPageService, PageService>();

    // Infrastructure Services
    builder.Services.AddScoped<DataSeedingService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    // CORS (if needed for API)
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();

    // JWT Cookie Middleware - reads JWT from cookies and adds to Authorization header
    app.UseMiddleware<JwtCookieMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    // Admin Authorization Middleware - protects admin routes
    app.UseMiddleware<AdminAuthorizationMiddleware>();

    app.MapRazorPages();

    // Map API controllers
    app.MapControllers();

    // Ensure database is created and seed data (for development only)
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        try
        {
            dbContext.Database.EnsureCreated();
            Log.Information("Database connection verified");

            // Seed roles if they don't exist
            if (!await dbContext.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role { Name = "SuperAdmin", Description = "Full system access" },
                    new Role { Name = "Admin", Description = "Content and enquiry management" },
                    new Role { Name = "Editor", Description = "Content management only" },
                    new Role { Name = "Support", Description = "Contact enquiries only" }
                };
                dbContext.Roles.AddRange(roles);
                await dbContext.SaveChangesAsync();
                Log.Information("Seeded default roles");
            }

            // Create admin user if it doesn't exist
            var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@taxhelpertoday.com");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = "admin@taxhelpertoday.com",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    FullName = "System Administrator",
                    IsActive = true,
                    IsEmailVerified = true
                };
                dbContext.Users.Add(adminUser);
                await dbContext.SaveChangesAsync();

                // Assign SuperAdmin role
                var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
                if (superAdminRole != null)
                {
                    dbContext.UserRoles.Add(new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = superAdminRole.Id
                    });
                    await dbContext.SaveChangesAsync();
                }
                Log.Information("Created default admin user: admin@taxhelpertoday.com / Admin@123");
            }

            // Seed initial content data if in development
            if (app.Environment.IsDevelopment())
            {
                var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
                    await seedingService.SeedDataAsync(adminUser.Id);
            }

            // Seed default SMTP/email settings (runs on first app start)
            var existingSettings = await dbContext.AdminSettings
                .Where(s => s.Key.StartsWith("smtp_"))
                .ToListAsync();
            var existingKeys = new HashSet<string>(existingSettings.Select(s => s.Key));

            var defaultSmtpSettings = new[]
            {
                new AdminSetting { Key = "smtp_enabled", Value = "false", Description = "Enable Email Sending" },
                new AdminSetting { Key = "smtp_host", Value = "smtpout.secureserver.net", Description = "SMTP Server Host" },
                new AdminSetting { Key = "smtp_port", Value = "25", Description = "SMTP Port" },
                new AdminSetting { Key = "smtp_username", Value = "support@taxhelpertoday.com", Description = "SMTP Username/Email" },
                new AdminSetting { Key = "smtp_password", Value = "Falc0n!Gold3n$", Description = "SMTP Password" },
                new AdminSetting { Key = "smtp_from_email", Value = "support@taxhelpertoday.com", Description = "From Email Address" },
                new AdminSetting { Key = "smtp_from_name", Value = "TaxHelperToday", Description = "From Name" },
                new AdminSetting { Key = "smtp_enable_ssl", Value = "false", Description = "Enable SSL/TLS" },
                new AdminSetting { Key = "smtp_admin_notification_email", Value = "support@taxhelpertoday.com", Description = "Admin Notification Email (where enquiry notifications are sent)" }
            };

            var newSmtpSettings = defaultSmtpSettings
                .Where(s => !existingKeys.Contains(s.Key))
                .ToList();

            if (newSmtpSettings.Any())
            {
                dbContext.AdminSettings.AddRange(newSmtpSettings);
                await dbContext.SaveChangesAsync();
                Log.Information("Seeded default SMTP/email settings");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error connecting to database. Please ensure PostgreSQL is running and connection string is correct.");
        }
    }

    Log.Information("TaxHelperToday application started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
