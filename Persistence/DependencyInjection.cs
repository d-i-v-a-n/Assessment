using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        //var assembly = typeof(DependencyInjection).Assembly;

        services.AddDbContext<ApplicationDbContext>();

        return services;
    }
}