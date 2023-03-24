namespace Domain;

public sealed class PasswordValidationError : Exception
{
    public PasswordValidationError() : base("Invalid password. Password length must be at least 8 characters.")
    {
    }
}
