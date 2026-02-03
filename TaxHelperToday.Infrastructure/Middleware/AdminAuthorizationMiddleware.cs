using System.Security.Claims;

namespace TaxHelperToday.Infrastructure.Middleware;

public class AdminAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminAuthorizationMiddleware> _logger;

    public AdminAuthorizationMiddleware(RequestDelegate next, ILogger<AdminAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply to admin routes
        if (context.Request.Path.StartsWithSegments("/Admin") && 
            !context.Request.Path.StartsWithSegments("/Admin/Login"))
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                // Not authenticated, redirect to login
                context.Response.Redirect("/Admin/Login");
                return;
            }

            // Check if user has admin role
            var hasAdminRole = context.User.IsInRole("SuperAdmin") ||
                              context.User.IsInRole("Admin") ||
                              context.User.IsInRole("Editor") ||
                              context.User.IsInRole("Support");

            if (!hasAdminRole)
            {
                // User doesn't have admin role, return 403
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied. Admin privileges required.");
                return;
            }
        }

        await _next(context);
    }
}
