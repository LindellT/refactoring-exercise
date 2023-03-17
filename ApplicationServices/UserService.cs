namespace ApplicationServices;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) => _userRepository = userRepository;

    public (bool success, int? id, Dictionary<string, string[]>? errors) CreateUser(string? email, string? password)
    {
        var validPassword = !string.IsNullOrEmpty(password) && password.Length > 7;

        var validationProblems = new List<(string field, string description)>();

        if (!validPassword)
        {
            validationProblems.Add(("password", "Invalid password. Password must be minimum of 8 characters."));
        }

        var validEmail = !string.IsNullOrEmpty(email) && email.Length > 2 && email.Contains('@');

        if (!validEmail)
        {
            validationProblems.Add(("email", "Invalid email. Email must have a recipient and domain and contain @ sign."));
        }

        if (validationProblems.Any())
        {
            return (false, null, validationProblems.ToDictionary(p => p.field, p => validationProblems.Where(vp => vp.field == p.field).Select(vp => vp.description).ToArray()));
        }

        if (_userRepository.FindUserByEmail(email) is not null)
        {
            return (false, null, new Dictionary<string, string[]>() { { "email", new string[] { "Email reserved." } } });
        }

        var user = _userRepository.CreateUser(email!, password!);

        if (user is null)
        {
            return (false, null, new Dictionary<string, string[]>() { { string.Empty, new string[] { "User creation failed" } } });
        }

        return (true, user.Id, null);
    }

    public bool DeleteUser(int id) => _userRepository.DeleteUser(id);

    public UserDTO? FindUser(int id) => UserDTO.FromUser(_userRepository.FindUser(id));

    public List<UserDTO> ListUsers() => _userRepository.ListUsers().Select(u => UserDTO.FromUser(u)!).ToList();

    public (bool success, string? error) UpdateUser(int id, string? email, string? password)
    {
        var validPassword = !string.IsNullOrEmpty(password) && password.Length > 7;
        var validEmail = !string.IsNullOrEmpty(email) && email.Length > 2 && email.Contains('@');

        if (!validPassword && !validEmail)
        {
            return (false, "Invalid email and password");
        }

        var user = _userRepository.FindUser(id);

        if (user is null)
        {
            return (false, "User doesn't exist");
        }

        var userByEmail = _userRepository.FindUserByEmail(email);

        if (userByEmail != null && userByEmail.Id != id)
        {
            return (false, "Email reserved.");
        }


        if (validEmail)
        {
            user = user with { Email = email! };
        }

        if (validPassword)
        {
            user = user with { Password = password! };
        }

        var success = _userRepository.UpdateUser(user);

        return (success, success ? null : "Updating user failed or no changes");
    }
}
