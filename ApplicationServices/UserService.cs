using Domain;

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

    public async Task<(bool success, int? id, string? error)> CreateUserAsync(ValidEmailAddress email, ValidPassword password, CancellationToken cancellationToken)
    {
        if (await _userRepository.FindUserByEmailAsync(email, cancellationToken) is not null)
        {
            return (false, null, "Email reserved.");
        }

        var hashedPassword = HashedPassword.CreateFrom(password, _validPasswordSalt);

        var user = await _userRepository.CreateUserAsync(email, hashedPassword, cancellationToken);

        if (user is null)
        {
            return (false, null, "User creation failed");
        }

        return (true, user.Id, null);
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken) => await _userRepository.DeleteUserAsync(id, cancellationToken);

    public async Task<UserDTO?> FindUserAsync(int id, CancellationToken cancellationToken) => UserDTO.FromUser(await _userRepository.FindUserAsync(id, cancellationToken));

    public async Task<List<UserDTO>> ListUsersAsync(CancellationToken cancellationToken) => (await _userRepository.ListUsersAsync(cancellationToken)).Select(u => UserDTO.FromUser(u)!).ToList();

    public async Task<(bool success, string? error)> UpdateUserAsync(int id, ValidEmailAddress? email, ValidPassword? password, CancellationToken cancellationToken)
    {
        if (email is null && password is null)
        {
            return (false, "Invalid email and password");
        }

        var user = await _userRepository.FindUserAsync(id, cancellationToken);

        if (user is null)
        {
            return (false, "User doesn't exist");
        }

        if (email is not null)
        {
            var userByEmail = await _userRepository.FindUserByEmailAsync(email, cancellationToken);

            if (userByEmail is not null && userByEmail.Id != id)
            {
                return (false, "Email reserved.");
            }

            user = user with { Email = email };
        }

        if (password is not null)
        {
            user = user with { HashedPassword = HashedPassword.CreateFrom(password, _validPasswordSalt) };
        }

        var success = await _userRepository.UpdateUserAsync(user, cancellationToken);

        return (success, success ? null : "Updating user failed or no changes");
    }
}
