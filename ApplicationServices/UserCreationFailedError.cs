namespace ApplicationServices;

public class UserCreationFailedError : Exception
{
    public UserCreationFailedError() : base("User creation failed.")
    {
    }
}