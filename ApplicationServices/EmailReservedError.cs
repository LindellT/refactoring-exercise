namespace ApplicationServices;

public sealed class EmailReservedError : Exception
{
    public EmailReservedError() : base("Email reserved.")
    {
    }
}