using Microsoft.Extensions.DependencyInjection;

namespace ApplicationServices;

public static class ServiceCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services) => services.AddTransient<IUserService, UserService>();
}