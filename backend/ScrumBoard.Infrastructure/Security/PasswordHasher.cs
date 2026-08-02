using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Infrastructure.Security;

public class PasswordHasher(IConfiguration configuration) : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private readonly string _pepper = configuration["Security:Pepper"]
                                      ?? throw new InvalidOperationException("Security:Pepper no configurado.");

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Derive(password, salt);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";

    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 3) return false;

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var storedHash = Convert.FromBase64String(parts[2]);

        var computedHash = Derive(password, salt, iterations);
        return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);

    }
    
    private byte[] Derive(string password, byte[] salt, int iterations = Iterations)
    {
        var passwordWithPepper = password + _pepper;
        return Rfc2898DeriveBytes.Pbkdf2(
            passwordWithPepper, salt, iterations, HashAlgorithmName.SHA256, KeySize);
    }
}