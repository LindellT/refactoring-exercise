using Domain;

namespace ApplicationServices;

public interface IUserRepository
{
    User? CreateUser(ValidEmailAddress email, HashedPassword hashedPassword);
    
    bool DeleteUser(int id);
    
    User? FindUser(int id);

    User? FindUserByEmail(ValidEmailAddress email);
    
    List<User> ListUsers();
    
    bool UpdateUser(User user);
}
