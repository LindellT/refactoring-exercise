namespace ApplicationServices;

public sealed class UserCreationFailedError : Exception
{
    public UserCreationFailedError() : base("User creation failed.")
    {
    }
}