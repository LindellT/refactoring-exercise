namespace Domain;

public sealed record ValidPasswordSalt
{
    private ValidPasswordSalt(string salt)
    {
        Salt = salt;
    }

    public string Salt { get; init; }

    public const string ValidationRequirements = "Invalid salt. Salt length must be at least 32 characters.";

    public static ValidPasswordSalt? CreateFrom(string salt)
    {
        if (salt.Length < 32)
        {
            return null;
        }

        return new ValidPasswordSalt(salt);
    }
}
