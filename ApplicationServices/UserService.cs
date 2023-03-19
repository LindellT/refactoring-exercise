using Domain;

namespace ApplicationServices;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ValidPasswordSalt _validPasswordSalt;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _validPasswordSalt = ValidPasswordSalt.CreateFrom("12345678901234567890123465789012")!;
    }

    public (bool success, int? id, string? error) CreateUser(ValidEmailAddress email, ValidPassword password)
    {
        if (_userRepository.FindUserByEmail(email) is not null)
        {
            return (false, null, "Email reserved.");
        }

        var hashedPassword = HashedPassword.CreateFrom(password, _validPasswordSalt);

        var user = _userRepository.CreateUser(email, hashedPassword);

        if (user is null)
        {
            return (false, null, "User creation failed");
        }

        return (true, user.Id, null);
    }

    public bool DeleteUser(int id) => _userRepository.DeleteUser(id);

    public UserDTO? FindUser(int id) => UserDTO.FromUser(_userRepository.FindUser(id));

    public List<UserDTO> ListUsers() => _userRepository.ListUsers().Select(u => UserDTO.FromUser(u)!).ToList();

    public (bool success, string? error) UpdateUser(int id, ValidEmailAddress? email, ValidPassword? password)
    {
        if (email is null && password is null)
        {
            return (false, "Invalid email and password");
        }

        var user = _userRepository.FindUser(id);

        if (user is null)
        {
            return (false, "User doesn't exist");
        }

        if (email is not null)
        {
            var userByEmail = _userRepository.FindUserByEmail(email);

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

        var success = _userRepository.UpdateUser(user);

        return (success, success ? null : "Updating user failed or no changes");
    }
}
