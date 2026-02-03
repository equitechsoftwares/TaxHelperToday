using BCrypt.Net;

namespace TaxHelperToday.Modules.Identity.Infrastructure.Services;

public class PasswordHasher
{
    private const int SaltRounds = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, SaltRounds);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
