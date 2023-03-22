namespace ApplicationServices;

public class EmailReservedError : Exception
{
    public EmailReservedError() : base("Email reserved.")
    {
    }
}