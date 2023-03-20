using ApplicationServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
    {
        services.AddDbContext<UserContext>(dbContextOptionsBuilder);
        services.AddTransient<IUserRepository, UserRepository>();
    }
}