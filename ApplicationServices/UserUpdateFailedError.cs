namespace ApplicationServices;

public sealed class UserUpdateFailedError : Exception
{
    public UserUpdateFailedError() : base("Updating user failed.")
    {
    }    
}
