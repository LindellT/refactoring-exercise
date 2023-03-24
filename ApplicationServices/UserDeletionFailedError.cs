namespace ApplicationServices;

public sealed class UserDeletionFailedError : Exception
{
    public UserDeletionFailedError() : base("User deletion failed.")
    {
    }
}