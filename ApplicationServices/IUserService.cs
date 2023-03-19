using Domain;

namespace ApplicationServices;

public interface IUserService
{
    List<UserDTO> ListUsers();

    UserDTO? FindUser(int id);

    bool DeleteUser(int id);

    (bool success, string? error) UpdateUser(int id, ValidEmailAddress? email, ValidPassword? password);

    (bool success, int? id, string? error) CreateUser(ValidEmailAddress email, ValidPassword password);
}
