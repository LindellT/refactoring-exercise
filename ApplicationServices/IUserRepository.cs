using Domain;
using OneOf;
using OneOf.Types;

namespace ApplicationServices;

public interface IUserRepository
{
    Task<OneOf<Success<int>, UserCreationFailedError>> CreateUserAsync(ValidEmailAddress email, HashedPassword hashedPassword, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken);
    
    Task<OneOf<User, NotFound>> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<OneOf<User, NotFound>> FindUserByEmailAsync(ValidEmailAddress email, CancellationToken cancellationToken);
    
    Task<List<User>> ListUsersAsync(CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound, UserUpdateFailedError>> UpdateUserAsync(User user, CancellationToken cancellationToken);
}