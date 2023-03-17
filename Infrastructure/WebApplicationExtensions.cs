using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceScopeExtensions
{
    public static async Task MigrateDatabaseAsync(this IServiceScope scope)
    {        
        var context = scope.ServiceProvider.GetRequiredService<UserContext>();
        await context.Database.MigrateAsync();
    }
}
