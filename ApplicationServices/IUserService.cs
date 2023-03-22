using Domain;
using OneOf.Types;
using OneOf;

namespace ApplicationServices;

public interface IUserService
{
    Task<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);

    Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken);

    Task<UserDTO?> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken);

    Task<(bool success, string? error)> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}