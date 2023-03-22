using Domain;
using OneOf;
using OneOf.Types;

namespace ApplicationServices;

public interface IUserRepository
{
    Task<OneOf<Success<int>, UserCreationFailedError>> CreateUserAsync(ValidEmailAddress email, HashedPassword hashedPassword, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken);
    
    Task<User?> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<User?> FindUserByEmailAsync(ValidEmailAddress email, CancellationToken cancellationToken);
    
    Task<List<User>> ListUsersAsync(CancellationToken cancellationToken);
    
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
}