using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaxHelperToday.Infrastructure.Data;
using TaxHelperToday.Modules.Identity.Application.Services;

namespace TaxHelperToday.Infrastructure.Middleware;

public class JwtCookieMiddleware
{
    private readonly RequestDelegate _next;

    public JwtCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if we have an access token in cookies
        var accessToken = context.Request.Cookies["access_token"];
        var refreshToken = context.Request.Cookies["refresh_token"];

        if (!string.IsNullOrEmpty(accessToken))
        {
            // Check if token is expired
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(accessToken))
            {
                var token = handler.ReadJwtToken(accessToken);
                
                if (token.ValidTo > DateTime.UtcNow)
                {
                    // Token is not expired, but check token version to see if it's been invalidated
                    var tokenVersionClaim = token.Claims.FirstOrDefault(c => c.Type == "token_version");
                    if (tokenVersionClaim != null && long.TryParse(tokenVersionClaim.Value, out var tokenVersion))
                    {
                        // Check if token version matches current user version
                        var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                        {
                            var user = await dbContext.Users.FindAsync(userId);
                            if (user != null && user.TokenVersion != tokenVersion)
                            {
                                // Token version mismatch - token has been invalidated
                                // Clear cookies
                                var cookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = context.Request.IsHttps,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTime.UtcNow.AddDays(-1)
                                };
                                context.Response.Cookies.Append("access_token", string.Empty, cookieOptions);
                                context.Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);
                                
                                // For admin routes, redirect to login instead of returning 401
                                if (context.Request.Path.StartsWithSegments("/Admin") && 
                                    !context.Request.Path.StartsWithSegments("/Admin/Login"))
                                {
                                    context.Response.Redirect("/Admin/Login");
                                    return;
                                }
                                // For API routes, let it continue to return 401
                            }
                            else if (user != null && user.TokenVersion == tokenVersion)
                            {
                                // Token version matches, token is valid
                                if (!context.Request.Headers.ContainsKey("Authorization"))
                                {
                                    context.Request.Headers.Append("Authorization", $"Bearer {accessToken}");
                                }
                            }
                        }
                    }
                    else
                    {
                        // No token version claim (old token), check if user has token version > 0
                        var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                        {
                            var user = await dbContext.Users.FindAsync(userId);
                            if (user != null && user.TokenVersion > 0)
                            {
                                // User has token versioning enabled but token doesn't have version - invalidate
                                var cookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = context.Request.IsHttps,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTime.UtcNow.AddDays(-1)
                                };
                                context.Response.Cookies.Append("access_token", string.Empty, cookieOptions);
                                context.Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);
                                
                                // For admin routes, redirect to login instead of returning 401
                                if (context.Request.Path.StartsWithSegments("/Admin") && 
                                    !context.Request.Path.StartsWithSegments("/Admin/Login"))
                                {
                                    context.Response.Redirect("/Admin/Login");
                                    return;
                                }
                                // For API routes, let it continue to return 401
                            }
                            else
                            {
                                // No token versioning yet, allow token
                                if (!context.Request.Headers.ContainsKey("Authorization"))
                                {
                                    context.Request.Headers.Append("Authorization", $"Bearer {accessToken}");
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Token is expired, try to refresh it
                    try
                    {
                        var identityService = context.RequestServices.GetRequiredService<IIdentityService>();
                        var refreshResult = await identityService.RefreshTokenAsync(refreshToken);

                        // Update cookies with new tokens
                        var accessTokenCookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = context.Request.IsHttps,
                            SameSite = SameSiteMode.Strict,
                            Expires = refreshResult.ExpiresAt // Match access token expiration
                        };

                        var refreshTokenCookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = context.Request.IsHttps,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddDays(7) // Refresh token expires in 7 days
                        };

                        context.Response.Cookies.Append("access_token", refreshResult.AccessToken, accessTokenCookieOptions);
                        context.Response.Cookies.Append("refresh_token", refreshResult.RefreshToken, refreshTokenCookieOptions);

                        // Add new token to Authorization header
                        if (!context.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Request.Headers.Append("Authorization", $"Bearer {refreshResult.AccessToken}");
                        }
                    }
                    catch
                    {
                        // Refresh failed (token revoked or expired), clear cookies
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = context.Request.IsHttps,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddDays(-1)
                        };
                        context.Response.Cookies.Append("access_token", string.Empty, cookieOptions);
                        context.Response.Cookies.Append("refresh_token", string.Empty, cookieOptions);
                        
                        // For admin routes, redirect to login instead of returning 401
                        if (context.Request.Path.StartsWithSegments("/Admin") && 
                            !context.Request.Path.StartsWithSegments("/Admin/Login"))
                        {
                            context.Response.Redirect("/Admin/Login");
                            return;
                        }
                        // For API routes, let it continue to return 401
                    }
                }
            }
        }

        await _next(context);
    }
}
