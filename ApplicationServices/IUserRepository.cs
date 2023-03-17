using Domain;

namespace ApplicationServices;

public interface IUserRepository
{
    User? CreateUser(string email, string password);
    
    bool DeleteUser(int id);
    
    User? FindUser(int id);

    User? FindUserByEmail(string? email);
    
    List<User> ListUsers();
    
    bool UpdateUser(User user);
}
