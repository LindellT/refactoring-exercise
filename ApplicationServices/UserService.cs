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
        User? userByEmail = null;
        (await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken)).Switch(res => userByEmail = res, _ => userByEmail = null);
        
        if (userByEmail is not null)
        {
            return new EmailReservedError();
        }

        var hashedPassword = HashedPassword.CreateFrom(command.Password, _validPasswordSalt);

        return (await _userRepository.CreateUserAsync(command.EmailAddress, hashedPassword, cancellationToken))
            .Match<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>>(success => success, error => error);
    }

    public async Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken)
        => await _userRepository.DeleteUserAsync(id, cancellationToken);

    public async Task<OneOf<UserDTO, NotFound>> FindUserAsync(int id, CancellationToken cancellationToken)
        => (await _userRepository.FindUserAsync(id, cancellationToken))
            .Match<OneOf<UserDTO, NotFound>>(
                user => UserDTO.FromUser(user),
                notFound => notFound);

    public async Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken)
        => (await _userRepository.ListUsersAsync(cancellationToken)).Select(u => UserDTO.FromUser(u)!).ToList();

    public async Task<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        User? user = null;
        (await _userRepository.FindUserAsync(command.Id, cancellationToken)).Switch(res => user = res, _ => user = null);

        if (user is null)
        {
            return new NotFound();
        }

        if (command.EmailAddress is not null)
        {
            User? userByEmail = null;
            (await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken)).Switch(res => userByEmail = res, _ => userByEmail = null);

            if (userByEmail is not null && userByEmail.Id != command.Id)
            {
                return new EmailReservedError();
            }

            user = user with { Email = command.EmailAddress, };
        }

        if (command.Password is not null)
        {
            user = user with { HashedPassword = HashedPassword.CreateFrom(command.Password, _validPasswordSalt), };
        }

        return (await _userRepository.UpdateUserAsync(user, cancellationToken))
            .Match<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>>(
                success => success,
                notFound => notFound,
                userUpdateFailedError => userUpdateFailedError);
    }
}