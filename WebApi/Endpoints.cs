using Application.Commands;
using Application.Queries;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using System.Security.Claims;
using WebApi.Contracts;

namespace WebApi;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            RegisterRequest request,
            UserManager<User> userManager,
            IConfiguration configuration,
            ISender sender,
            CancellationToken ct) =>
        {
            var user = new User()
            {
                Email = request.Email,
                UserName = request.Email
            };

            var createUserResult = await userManager.CreateAsync(user, request.Password);
            if(!createUserResult.Succeeded)
                return Results.BadRequest(createUserResult.Errors);
            
            return Results.Ok();
        });
        // todo: moderator make a user as moderator

        app.MapPost("/login", async (
            LoginRequest request,
            UserManager<User> userManager, 
            IConfiguration configuration, 
            ISender sender, 
            CancellationToken ct) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, request.Password))
            {
                var roles = await userManager.GetRolesAsync(user);

                var token = JwtGenerator.GenerateJwtToken(user, roles.ToArray(), configuration); // Create a JWT token for the user
                return Results.Ok(new { token });
            }

            return Results.Unauthorized();
        });


        //app.MapGet("/test", () => {
        //    return "Cool!";
        //});

        //app.MapGet("/test-auth", (ApplicationDbContext context, ClaimsPrincipal user) => {
        //    return $"Hi, {user.Identity.Name}";
        //}).RequireAuthorization();

        //app.MapGet("/test-auth-moderator",
        //    [Authorize(Roles = Roles.Moderator)]
        //    (ApplicationDbContext context, ClaimsPrincipal user) => {
        //    return $"Hi, {user.Identity.Name}";
        //}).RequireAuthorization();


        app.MapGet("/posts", async (
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? authorEmail,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string[]? tags,
            [FromQuery] string? sortBy,
            ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPostsQuery(
                PageNumber: pageNumber ?? 1,
                PageSize: pageSize ?? 10,
                AuthorEmail: authorEmail,
                StartDateUtc: startDate,
                EndDateUtc: endDate,
                Tags: tags,
                SortBy: sortBy), ct);

            return result;
        });
        app.MapGet("/posts/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPostQuery(id), ct);

            return result;
        });

        app.MapPost("/posts", async (AddPostRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddPostCommand(Content: request.Content);

            var result = await sender.Send(command, ct);

            return result;
        }).RequireAuthorization();

        app.MapPost("/posts/{id:int}/like", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new LikePostCommand(PostId: id);

            await sender.Send(command, ct);

        }).RequireAuthorization();

        app.MapPost("/posts/{id:int}/comment", async (int id, CommentOnPostRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CommentOnPostCommand(PostId: id, request.Comment);

            await sender.Send(command, ct);

        }).RequireAuthorization();

        app.MapPost("/moderator/posts/{id:int}/tag",
            [Authorize(Roles = Roles.Moderator)]
            async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new TagPostCommand(PostId: id, "misleading or false information");

            await sender.Send(command, ct);

        }).RequireAuthorization();

        app.MapDelete("/posts/{id:int}/like", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new DeletePostLikeCommand(PostId: id);

            await sender.Send(command, ct);

        }).RequireAuthorization();
    }
}
