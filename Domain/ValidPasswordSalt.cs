using OneOf;

namespace Domain;

public sealed record ValidPasswordSalt
{
    private ValidPasswordSalt(string salt)
    {
        Salt = salt;
    }

    public string Salt { get; init; }

    public static OneOf<ValidPasswordSalt, PasswordSaltValidationError> CreateFrom(string salt)
    {
        if (salt.Length < 32)
        {
            return new PasswordSaltValidationError();
        }

        return new ValidPasswordSalt(salt);
    }
}