using Domain;

namespace ApplicationServices;

public record UserDTO(int Id, string Email)
{
    public static explicit operator UserDTO?(User? u)
    {
        if (u == null)
        {
            return null;
        }

        return new(u.Id, u.Email);
    }

    public static UserDTO? FromUser(User? u) => (UserDTO?)u;
}