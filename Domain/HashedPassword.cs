using System.Security.Cryptography;
using System.Text;

namespace Domain;

public sealed record HashedPassword
{
#pragma warning disable CS0628 // New protected member declared in sealed type, used by EF Core
    protected HashedPassword() { }
#pragma warning restore CS0628 // New protected member declared in sealed type

    private HashedPassword(string hashedPassword)
    {
        Hash = hashedPassword;
    }

    public string Hash { get; init; } = null!;

    public static HashedPassword CreateFrom(ValidPassword password, ValidPasswordSalt salt)
    {
        using var sha = SHA256.Create();
        
        // Convert the string to a byte array first, to be processed
        byte[] textBytes = Encoding.UTF8.GetBytes(password.Password + salt.Salt);
        byte[] hashBytes = SHA256.HashData(textBytes);

        // Convert back to a string, removing the '-' that BitConverter adds
        string hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);

        return new HashedPassword(hash);
    }
}