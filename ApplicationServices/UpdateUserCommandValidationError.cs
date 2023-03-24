namespace ApplicationServices;

public class UpdateUserCommandValidationError : Exception
{
    public UpdateUserCommandValidationError() : base("Either email or password has to be valid.")
    {
    }
}
