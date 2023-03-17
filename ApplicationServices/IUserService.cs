namespace ApplicationServices;

public interface IUserService
{
    List<UserDTO> ListUsers();

    UserDTO? FindUser(int id);

    bool DeleteUser(int id);

    (bool success, string? error) UpdateUser(int id, string? email, string? password);

    (bool success, int? id, Dictionary<string, string[]>? errors) CreateUser(string? email, string? password);
}
