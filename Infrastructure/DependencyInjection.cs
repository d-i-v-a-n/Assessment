﻿using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //var assembly = typeof(DependencyInjection).Assembly;

        return services;
    }
}