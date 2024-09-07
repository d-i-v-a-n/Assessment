using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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


        app.MapGet("/posts", async (
            //[FromQuery] GetPostsRequest request, 
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? authorEmail,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string[]? tags,
            [FromQuery] string? sortBy,
            ISender sender, CancellationToken ct) =>
        {
            //var result = await sender.Send(new GetPostsQuery(
            //    PageNumber: request.PageNumber,
            //    PageSize: request.PageSize,
            //    AuthorEmail: request.AuthorEmail,
            //    StartDate: request.StartDate,
            //    EndDate: request.EndDate,
            //    Tags: request.Tags,
            //    SortBy: request.SortBy), ct);
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
        });//.RequireAuthorization();

        app.MapPost("/posts/{id:int}/like", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new LikePostCommand(PostId: id);

            await sender.Send(command, ct);

        });//.RequireAuthorization();
        app.MapPost("/posts/{id:int}/comment", async (int id, CommentOnPostRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CommentOnPostCommand(PostId: id, request.Comment);

            await sender.Send(command, ct);

        });//.RequireAuthorization();

        app.MapPost("/moderator/posts/{id:int}/tag", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new TagPostCommand(PostId: id, "misleading or false information");

            await sender.Send(command, ct);

        });//.RequireAuthorization();

        app.MapDelete("/posts/{id:int}/like", async (int id, ISender sender, CancellationToken ct) =>
        {
            var command = new DeletePostLikeCommand(PostId: id);

            await sender.Send(command, ct);

        });//.RequireAuthorization();
    }
}
