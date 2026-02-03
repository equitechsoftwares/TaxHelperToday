using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Modules.Identity.Domain.Entities;
using TaxHelperToday.Modules.Content.Domain.Entities;
using TaxHelperToday.Modules.Contact.Domain.Entities;
using TaxHelperToday.Modules.Admin.Domain.Entities;

namespace TaxHelperToday.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Identity Module
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Content Module
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogTag> BlogTags { get; set; }
    public DbSet<BlogPostTag> BlogPostTags { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<Page> Pages { get; set; }

    // Contact Module
    public DbSet<ContactEnquiry> ContactEnquiries { get; set; }
    public DbSet<MiniEnquiry> MiniEnquiries { get; set; }

    // Admin Module
    public DbSet<AdminSetting> AdminSettings { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
