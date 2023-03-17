using Domain;

namespace Infrastructure;

internal class UserEntity
{ 
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public static implicit operator User?(UserEntity? u) => u is not null ? new(u.Id, u.Email, u.Password) : null;
}