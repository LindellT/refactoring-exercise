﻿using Domain;

namespace Infrastructure;

internal class UserEntity
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

        return u is not null ? new(u.Id, u.Email, u.HashedPassword) : null;
    }
}