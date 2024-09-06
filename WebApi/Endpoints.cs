using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Persistence;
using System.Security.Claims;
using WebApi.Contracts;

namespace WebApi;

public static class Endpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/test", () => {
            return "Cool!";
        });

        app.MapGet("/test-auth", (ApplicationDbContext context, ClaimsPrincipal user) => {
            return $"Hi, {user.Identity.Name}";
        }).RequireAuthorization();


        app.MapGet("/posts", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPostsQuery(), ct);

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

        app.MapDelete("/posts/{id:int}/like", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new DeletePostLikeCommand(PostId: id);

            await sender.Send(command, ct);

        }).RequireAuthorization();
    }
}
