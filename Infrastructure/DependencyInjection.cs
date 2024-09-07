using Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Security.Principal;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //var assembly = typeof(DependencyInjection).Assembly;

        return services;
    }
}

public static class Extensions
{
    public static /*IAuthenticated*/ AuthenticatedUser GetUser(this IIdentity identity)
    {
        var claimsIdentity = identity.GetClaimsIdentity();

        //var userUid = claimsIdentity.GetValue(Constants.ClaimTypes.UserUid) ?? throw new Exception($"Expected claim type '{Constants.ClaimTypes.UserUid}' not found.");
        

        return new AuthenticatedUser(
            id: claimsIdentity.GetValue(ClaimTypes.NameIdentifier),
            email: claimsIdentity.GetValue(ClaimTypes.Email),
            isAuthenticated: true,
            isModerator: false
            );
    }

    private static ClaimsIdentity GetClaimsIdentity(this IIdentity identity)
    {
        if (identity is ClaimsIdentity claimsIdentity)
            return claimsIdentity;

        throw new Exception("Not a ClaimsIdentity");
    }

    private static string? GetValue(this ClaimsIdentity claimsIdentity, string type) =>
        claimsIdentity.Claims.FirstOrDefault(c => c.Type == type)?.Value;
}