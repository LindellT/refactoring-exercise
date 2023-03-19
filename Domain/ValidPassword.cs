namespace Domain;

public record ValidPassword
{
    private ValidPassword(string password)
    {
        Password = password;
    }

    public string Password { get; init; }

    public const string ValidationRequirements = "Invalid password. Password length must be at least 8 characters.";

    public static ValidPassword? CreateFrom(string password)
    {
        if (password.Length < 8)
        {
            return null;
        }

        return new ValidPassword(password);
    }
}
