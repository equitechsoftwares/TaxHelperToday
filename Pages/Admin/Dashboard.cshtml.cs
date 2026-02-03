using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxHelperToday.Infrastructure.Data;

namespace TaxHelperToday.Pages.Admin;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(ApplicationDbContext context, ILogger<DashboardModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public int TotalBlogs { get; set; }
    public int PublishedBlogs { get; set; }
    public int DraftBlogs { get; set; }
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public int InactiveServices { get; set; }
    public int TotalEnquiries { get; set; }
    public int PendingEnquiries { get; set; }
    public int InProgressEnquiries { get; set; }
    public int ResolvedEnquiries { get; set; }
    public int TotalFaqs { get; set; }
    public int ActiveFaqs { get; set; }
    public int TotalMiniEnquiries { get; set; }
    public int NewMiniEnquiries { get; set; }
    public int TotalPages { get; set; }
    public int TotalUsers { get; set; }
    public int EnquiriesThisMonth { get; set; }
    public int MiniEnquiriesThisMonth { get; set; }
    public int BlogsThisMonth { get; set; }

    public List<BlogPostSummary> RecentBlogs { get; set; } = new();
    public List<EnquirySummary> RecentEnquiries { get; set; } = new();
    public List<MiniEnquirySummary> RecentMiniEnquiries { get; set; } = new();

    public class BlogPostSummary
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class EnquirySummary
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class MiniEnquirySummary
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public async Task OnGetAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Get blog statistics
            TotalBlogs = await _context.BlogPosts.CountAsync();
            PublishedBlogs = await _context.BlogPosts
                .CountAsync(b => b.IsPublished);
            DraftBlogs = TotalBlogs - PublishedBlogs;
            BlogsThisMonth = await _context.BlogPosts
                .CountAsync(b => b.CreatedAt >= startOfMonth);

            // Get service statistics
            TotalServices = await _context.Services.CountAsync();
            ActiveServices = await _context.Services
                .CountAsync(s => s.IsActive);
            InactiveServices = TotalServices - ActiveServices;

            // Get enquiry statistics
            TotalEnquiries = await _context.ContactEnquiries.CountAsync();
            PendingEnquiries = await _context.ContactEnquiries
                .CountAsync(e => e.Status == "Pending");
            InProgressEnquiries = await _context.ContactEnquiries
                .CountAsync(e => e.Status == "In Progress");
            ResolvedEnquiries = await _context.ContactEnquiries
                .CountAsync(e => e.Status == "Resolved");
            EnquiriesThisMonth = await _context.ContactEnquiries
                .CountAsync(e => e.CreatedAt >= startOfMonth);

            // Get FAQ statistics
            TotalFaqs = await _context.Faqs.CountAsync();
            ActiveFaqs = await _context.Faqs
                .CountAsync(f => f.IsActive);

            // Get mini enquiry statistics
            TotalMiniEnquiries = await _context.MiniEnquiries.CountAsync();
            NewMiniEnquiries = await _context.MiniEnquiries
                .CountAsync(e => e.Status == "New");
            MiniEnquiriesThisMonth = await _context.MiniEnquiries
                .CountAsync(e => e.CreatedAt >= startOfMonth);

            // Get page and user statistics
            TotalPages = await _context.Pages.CountAsync();
            TotalUsers = await _context.Users.CountAsync();

            // Get recent blog posts
            RecentBlogs = await _context.BlogPosts
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new BlogPostSummary
                {
                    Id = b.Id,
                    Title = b.Title,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            // Get recent enquiries
            RecentEnquiries = await _context.ContactEnquiries
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .Select(e => new EnquirySummary
                {
                    Id = e.Id,
                    Name = e.Name,
                    Subject = e.Subject ?? "No Subject",
                    Status = e.Status,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            // Get recent mini enquiries
            RecentMiniEnquiries = await _context.MiniEnquiries
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .Select(e => new MiniEnquirySummary
                {
                    Id = e.Id,
                    Name = e.Name,
                    Email = e.Email,
                    UserType = e.UserType ?? "N/A",
                    Status = e.Status,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
        }
    }
}
