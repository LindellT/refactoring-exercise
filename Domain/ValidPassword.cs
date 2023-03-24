using OneOf;

namespace Domain;

public sealed record ValidPassword
{
    private ValidPassword(string password)
    {
        Password = password;
    }

    public string Password { get; init; }

    public static OneOf<ValidPassword, PasswordValidationError> CreateFrom(string? password)
    {
        if (password is null || password.Length < 8)
        {
            return new PasswordValidationError();
        }

        return new ValidPassword(password);
    }
}