using Domain;

namespace ApplicationServices;

public interface IUserService
{
    Task<(bool success, int? id, string? error)> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);

    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken);

    Task<UserDTO?> FindUserAsync(int id, CancellationToken cancellationToken);

    Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken);

    Task<(bool success, string? error)> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}