using Domain;

namespace ApplicationServices;

public sealed record UserDTO(int Id, string Email)
{
    public static explicit operator UserDTO(User u)
    {
        ArgumentNullException.ThrowIfNull(u);

        return new(u.Id, u.Email.Address);
    }

    public static UserDTO FromUser(User u) => (UserDTO)u;
}