using Domain;
using OneOf;
using OneOf.Types;

namespace ApplicationServices;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ValidPasswordSalt _validPasswordSalt;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _validPasswordSalt = ValidPasswordSalt.CreateFrom("12345678901234567890123465789012")!;
    }

    public async Task<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken) is not null)
        {
            return new EmailReservedError();
        }

        var hashedPassword = HashedPassword.CreateFrom(command.Password, _validPasswordSalt);

        return (await _userRepository.CreateUserAsync(command.EmailAddress, hashedPassword, cancellationToken))
            .TryPickT0(out var success, out var error) ? success : error;
    }

    public async Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken) => await _userRepository.DeleteUserAsync(id, cancellationToken);

    public async Task<UserDTO?> FindUserAsync(int id, CancellationToken cancellationToken) => UserDTO.FromUser(await _userRepository.FindUserAsync(id, cancellationToken));

    public async Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken) => (await _userRepository.ListUsersAsync(cancellationToken)).Select(u => UserDTO.FromUser(u)!).ToList();

    public async Task<(bool success, string? error)> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserAsync(command.Id, cancellationToken);

        if (user is null)
        {
            return (false, "User doesn't exist");
        }

        if (command.EmailAddress is not null)
        {
            var userByEmail = await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken);

            if (userByEmail is not null && userByEmail.Id != command.Id)
            {
                return (false, "Email reserved.");
            }

            user = user with { Email = command.EmailAddress, };
        }

        if (command.Password is not null)
        {
            user = user with { HashedPassword = HashedPassword.CreateFrom(command.Password, _validPasswordSalt), };
        }

        var success = await _userRepository.UpdateUserAsync(user, cancellationToken);

        return (success, success ? null : "Updating user failed or no changes");
    }
}