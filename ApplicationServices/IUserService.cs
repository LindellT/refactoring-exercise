using OneOf.Types;
using OneOf;

namespace ApplicationServices;

public interface IUserService
{
    Task<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);

    Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken);

    Task<OneOf<UserDTO, NotFound>> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken);

    Task<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}