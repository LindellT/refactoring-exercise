using ApplicationServices;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

internal sealed class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<User?> CreateUserAsync(ValidEmailAddress email, HashedPassword passwordHash, CancellationToken cancellationToken)
    {
        var user = new UserEntity
        {
            Email = email,
            HashedPassword = passwordHash,
        };
            
        await _userContext.AddAsync(user, cancellationToken);

        return await _userContext.SaveChangesAsync(cancellationToken) != 0 ? user : null;
}

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;

        return await _userContext.SaveChangesAsync(cancellationToken) == 1;
    }

    public async Task<User?> FindUserAsync(int id, CancellationToken cancellationToken) => await _userContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> FindUserByEmailAsync(ValidEmailAddress email, CancellationToken cancellationToken)
        => await _userContext.Users.FirstOrDefaultAsync(u => u.Email.Address == email.Address, cancellationToken);

    public async Task<List<User>> ListUsersAsync(CancellationToken cancellationToken) => await _userContext.Users.Select(u => (User)u!).ToListAsync(cancellationToken);

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (userEntity is null)
        {
            return false;
        }

        userEntity.Email = user.Email;
        userEntity.HashedPassword = user.HashedPassword;

        return await _userContext.SaveChangesAsync(cancellationToken) == 1;        
    }
}