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

            //public static async Task CreateUsersAndRoles(this IServiceProvider serviceProvider)
            //{
            //initializing custom roles 
            //var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = [$"{Roles.Member}", $"{Roles.Moderator}"];
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            var moderator = new User
            {
                UserName = "moderator@thisapp.co.za", //Configuration["AppSettings:UserName"],
                Email = "moderator@thisapp.co.za",
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = "P@ssw0rd";//Configuration["AppSettings:UserPassword"];
            var _user = await UserManager.FindByEmailAsync(moderator.Email);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(moderator, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(moderator, $"{Roles.Moderator}");

                }
            }
        }
    }
}
