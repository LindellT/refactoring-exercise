using Domain;

namespace ApplicationServices;

internal sealed class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ValidPasswordSalt _validPasswordSalt = ValidPasswordSalt.CreateFrom("12345678901234567890123465789012")
        .Match(
            validPasswordSalt => validPasswordSalt,
            passwordSaltValidationError => throw passwordSaltValidationError);

    public async Task<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
        => await (await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken))
            .Match<Task<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>>>(
                async found => await Task.FromResult(new EmailReservedError()),
                async _ => (await PersistNewUserAsync(command, cancellationToken))
                    .Match<OneOf<Success<int>, EmailReservedError, UserCreationFailedError>>(
                        success => success,
                        error => error)
                );

    private async Task<OneOf<Success<int>, UserCreationFailedError>> PersistNewUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
        => await _userRepository.CreateUserAsync(
            command.EmailAddress,
            HashedPassword.CreateFrom(command.Password, _validPasswordSalt),
            cancellationToken);

    public async Task<OneOf<Success, NotFound, UserDeletionFailedError>> DeleteUserAsync(int id, CancellationToken cancellationToken)
        => await _userRepository.DeleteUserAsync(id, cancellationToken);

    public async Task<OneOf<UserDTO, NotFound>> FindUserAsync(int id, CancellationToken cancellationToken)
        => (await _userRepository.FindUserAsync(id, cancellationToken))
            .Match<OneOf<UserDTO, NotFound>>(
                user => UserDTO.FromUser(user),
                notFound => notFound);

    public async Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken)
        => (await _userRepository.ListUsersAsync(cancellationToken))
            .Select(u => UserDTO.FromUser(u)!)
            .ToList();

    public async Task<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        => await (await _userRepository.FindUserAsync(command.Id, cancellationToken))
            .Match<Task<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>>>(
                async user => await (await CreateUpdatedUserToPersist(command, user, cancellationToken))
                    .Match<Task<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>>>(
                        async updatedUser => (await _userRepository.UpdateUserAsync(updatedUser, cancellationToken))
                            .Match<OneOf<Success, NotFound, EmailReservedError, UserUpdateFailedError>>(
                                success => success,
                                notFound => notFound,
                                userUpdateFailedError => userUpdateFailedError),
                        async emailReservedError => await Task.FromResult(emailReservedError)),
                async _ => await Task.FromResult(new NotFound()));

    private async Task<OneOf<User, EmailReservedError>> CreateUpdatedUserToPersist(UpdateUserCommand command, User user, CancellationToken cancellationToken)
    {
        if (command.EmailAddress is not null)
        {
            User? userByEmail = null;
            (await _userRepository.FindUserByEmailAsync(command.EmailAddress, cancellationToken))
                .Switch(
                    res => userByEmail = res,
                    _ => userByEmail = null);

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

        return user;
    }
}