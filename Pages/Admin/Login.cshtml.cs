using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaxHelperToday.Modules.Identity.Application.DTOs;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IIdentityService identityService, ILogger<LoginModel> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool Remember { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // If already logged in, redirect to dashboard
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect("/Admin/Dashboard");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please provide valid email and password.";
            return Page();
        }

        try
        {
            var loginDto = new LoginDto
            {
                Email = Email,
                Password = Password
            };

            var result = await _identityService.LoginAsync(loginDto);

            // Store tokens in cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = Remember ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(15)
            };

            Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", result.RefreshToken, cookieOptions);

            _logger.LogInformation("User {Email} logged in successfully", Email);

            // Redirect to dashboard
            return RedirectToPage("/Admin/Dashboard");
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = "Invalid email or password.";
            _logger.LogWarning("Failed login attempt for {Email}: {Error}", Email, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred. Please try again.";
            _logger.LogError(ex, "Error during login for {Email}", Email);
            return Page();
        }
    }
}
