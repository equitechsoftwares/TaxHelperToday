using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Identity.Application.DTOs;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin;

[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ChangePasswordModel> _logger;

    public ChangePasswordModel(IIdentityService identityService, ILogger<ChangePasswordModel> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [BindProperty]
    public ChangePasswordDto ChangePassword { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Admin/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var success = await _identityService.ChangePasswordAsync(userId.Value, ChangePassword);
            if (success)
            {
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToPage("/Admin/Profile");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to change password. Please try again.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            ModelState.AddModelError(string.Empty, $"Error changing password: {ex.Message}");
        }

        return Page();
    }

    private long? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && long.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
