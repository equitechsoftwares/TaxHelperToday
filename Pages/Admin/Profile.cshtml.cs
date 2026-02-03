using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Identity.Application.DTOs;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(IIdentityService identityService, ILogger<ProfileModel> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [BindProperty]
    public UpdateProfileDto Profile { get; set; } = new();

    public UserDto? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Admin/Login");
        }

        CurrentUser = await _identityService.GetUserByIdAsync(userId.Value);
        if (CurrentUser == null)
        {
            return NotFound();
        }

        // Populate the form with existing data
        Profile.Email = CurrentUser.Email;
        Profile.FullName = CurrentUser.FullName;
        Profile.Phone = CurrentUser.Phone ?? string.Empty;

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
            CurrentUser = await _identityService.GetUserByIdAsync(userId.Value);
            if (CurrentUser == null)
            {
                return NotFound();
            }
            return Page();
        }

        try
        {
            var success = await _identityService.UpdateProfileAsync(userId.Value, Profile);
            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile. Please try again.");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            ModelState.AddModelError(string.Empty, $"Error updating profile: {ex.Message}");
        }

        CurrentUser = await _identityService.GetUserByIdAsync(userId.Value);
        if (CurrentUser == null)
        {
            return NotFound();
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
