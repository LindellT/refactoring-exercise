using Domain;

namespace Infrastructure;

internal sealed class UserEntity
{ 
    public int Id { get; set; }

    public ValidEmailAddress Email { get; set; } = null!;

    public HashedPassword HashedPassword { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public static implicit operator User?(UserEntity? u)
    {
        if (u is null)
        {
            return null;
        }

        return new(u.Id, u.Email, u.HashedPassword);
    }
}