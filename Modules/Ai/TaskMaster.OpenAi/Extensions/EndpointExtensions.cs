using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.Base;

namespace TaskMaster.OpenAi.Extensions;

internal static class EndpointExtensions
{
    internal static void MapPostEndpoint<TQuery, TResponse, TExercise>(this IEndpointRouteBuilder app, string route,
        string endpoint, string tag)
        where TQuery : class, IQuery<TResponse> where TResponse : ExerciseResponse<TExercise> where TExercise : Exercise
    {
        app.MapPost($"/{route}/{endpoint}", async (TQuery query, IQueryDispatcher queryDispatcher) =>
            {
                var response = await queryDispatcher.QueryAsync(query);
                return Results.Ok(response);
            })
            //.RequireAuthorization()
            .Produces<TResponse>()
            .WithTags(tag);
    }
}