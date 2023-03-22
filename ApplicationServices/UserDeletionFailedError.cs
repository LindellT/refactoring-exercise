namespace ApplicationServices;

public class UserDeletionFailedError : Exception
{
    public UserDeletionFailedError() : base("User deletion failed.")
    {
    }
}