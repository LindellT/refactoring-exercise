namespace Domain;

public sealed class EmailValidationError : Exception
{
    public EmailValidationError() : base("Invalid email. Email must have a recipient and domain and contain @ sign.")
    { 
    }
}