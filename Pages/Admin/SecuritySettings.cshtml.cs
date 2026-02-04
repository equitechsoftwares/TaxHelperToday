using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin
{
    public class SecuritySettingsModel : PageModel
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<SecuritySettingsModel> _logger;

        public SecuritySettingsModel(
            IIdentityService identityService,
            ILogger<SecuritySettingsModel> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAllSessionsAsync()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    TempData["ErrorMessage"] = "Unable to identify user.";
                    return RedirectToPage();
                }

                var revokedCount = await _identityService.RevokeAllRefreshTokensAsync(userId);
                
                _logger.LogInformation("User {UserId} revoked {Count} active sessions", userId, revokedCount);
                
                TempData["SuccessMessage"] = $"Successfully logged out from {revokedCount} active session(s). You will need to log in again on other devices.";
                
                // Also clear current session cookies
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all sessions for user");
                TempData["ErrorMessage"] = "An error occurred while revoking sessions. Please try again.";
                return RedirectToPage();
            }
        }
    }
}
