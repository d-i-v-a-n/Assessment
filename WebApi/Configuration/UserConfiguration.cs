using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Configuration;

public static class UserConfiguration
{
    public static async Task CreateUsersAndRoles(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();

        using (var scope = serviceProvider.CreateScope())
        {
            var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = [Roles.Member, Roles.Moderator];
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
            }

            var moderator = new User
            {
                UserName = "moderator@thisapp.co.za",
                Email = "moderator@thisapp.co.za",
            };
            
            string userPWD = "P@ssw0rd";
            var _user = await UserManager.FindByEmailAsync(moderator.Email);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(moderator, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(moderator, Roles.Moderator);
                }
            }
        }
    }
}
