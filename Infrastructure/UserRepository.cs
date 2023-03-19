using ApplicationServices;
using Domain;
using System.Text.Json;

namespace Infrastructure;

internal class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    public User? CreateUser(ValidEmailAddress email, HashedPassword passwordHash)
    {
        var user = new UserEntity
        {
            Email = email,
            HashedPassword = passwordHash,
        };
            
        _userContext.Add(user);

        return _userContext.SaveChanges() != 0 ? user : null;
}

    public bool DeleteUser(int id)
    {
        var user = _userContext.Users.Find(id);

        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;

        return _userContext.SaveChanges() == 1;
    }

    public User? FindUser(int id) => FindUserEntity(id);

    public User? FindUserByEmail(ValidEmailAddress email) => _userContext.Users.FirstOrDefault(u => u.Email.Address == email.Address && !u.IsDeleted);

    public List<User> ListUsers() => _userContext.Users.Where(u => !u.IsDeleted).Select(u => (User)u!).ToList();

    public bool UpdateUser(User user)
    {
        var userEntity = FindUserEntity(user.Id);

        if (userEntity is null)
        {
            return false;
        }

        userEntity.Email = user.Email;
        userEntity.HashedPassword = user.HashedPassword;

        return _userContext.SaveChanges() == 1;        
    }

    private UserEntity? FindUserEntity(int id) => _userContext.Users.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
}
