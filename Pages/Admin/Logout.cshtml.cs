using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(IIdentityService identityService, ILogger<LogoutModel> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Get refresh token from cookie before deleting
            var refreshToken = Request.Cookies["refresh_token"];

            // Revoke refresh token if it exists
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _identityService.RevokeRefreshTokenAsync(refreshToken);
            }

            // Clear cookies with matching options (must match login cookie options)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1) // Set to past date to delete
            };

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            
            // Also explicitly set expired cookies to ensure deletion
            Response.Cookies.Append("access_token", string.Empty, cookieOptions);
            Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);

            // Sign out from authentication scheme (JWT is stateless, but this clears the user context)
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);

            _logger.LogInformation("User {Email} logged out successfully", User.Identity?.Name);

            // Redirect to login
            return RedirectToPage("/Admin/Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {Email}", User.Identity?.Name);
            
            // Even if there's an error, try to clear cookies and redirect
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append("access_token", string.Empty, cookieOptions);
            Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);
            
            return RedirectToPage("/Admin/Login");
        }
    }

    public IActionResult OnGet()
    {
        // If accessed via GET, show the logout page which will auto-submit POST
        return Page();
    }
}
