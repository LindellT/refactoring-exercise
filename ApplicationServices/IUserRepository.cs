using Domain;

namespace ApplicationServices;

public interface IUserRepository
{
    Task<User?> CreateUserAsync(ValidEmailAddress email, HashedPassword hashedPassword, CancellationToken cancellationToken);
    
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);
    
    Task<User?> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<User?> FindUserByEmailAsync(ValidEmailAddress email, CancellationToken cancellationToken);
    
    Task<List<User>> ListUsersAsync(CancellationToken cancellationToken);
    
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
}