using TaxHelperToday.Modules.Identity.Application.DTOs;

namespace TaxHelperToday.Modules.Identity.Application.Services;

public interface IIdentityService
{
    Task<LoginResultDto> LoginAsync(LoginDto loginDto);
    Task<TokenRefreshResultDto> RefreshTokenAsync(string refreshToken);
    Task<bool> RegisterAsync(RegisterDto registerDto);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task<int> RevokeAllRefreshTokensAsync(long userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetUserByIdAsync(long userId);
    Task<bool> UpdateProfileAsync(long userId, UpdateProfileDto updateProfileDto);
    Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto changePasswordDto);
}
