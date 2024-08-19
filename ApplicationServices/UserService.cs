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
        => (await UpdateEmailAddress(user, command.EmailAddress, cancellationToken))
            .Match<OneOf<User, EmailReservedError>>(
                updatedUser => UpdatePassword(updatedUser, command.Password),
                emailReservedError => emailReservedError);

    private async Task<OneOf<User, EmailReservedError>> UpdateEmailAddress(User user, ValidEmailAddress? validEmailAddress, CancellationToken cancellationToken)
        => validEmailAddress is null
            ? user
            : (await _userRepository.FindUserByEmailAsync(validEmailAddress, cancellationToken))
                .Match<OneOf<User, EmailReservedError>>(
                    res => new EmailReservedError(),
                    _ => user with { Email = validEmailAddress, });

    private User UpdatePassword(User user, ValidPassword? validPassword)
        => validPassword is null
            ? user
            : user with { HashedPassword = HashedPassword.CreateFrom(validPassword, _validPasswordSalt), };
}