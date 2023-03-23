using ApplicationServices;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api;

internal static class UserEndpointsV1
{
    public static RouteGroupBuilder MapUsersApiV1(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateUserAsync)
            .WithName(nameof(CreateUserAsync))
            .WithOpenApi();

        group.MapPut("/{id:int}", UpdateUserAsync)
            .WithName(nameof(UpdateUserAsync))
            .WithOpenApi();

        group.MapDelete("/{id:int}", DeleteUserAsync)
            .WithName(nameof(DeleteUserAsync))
            .WithOpenApi();

        group.MapGet("/", GetUsersAsync)
            .WithName(nameof(GetUsersAsync))
            .WithOpenApi();

        group.MapGet("/{id:int}", GetUserByIdAsync)
            .WithName(nameof(GetUserByIdAsync))
            .WithOpenApi();

        return group;
    }

    internal static async Task<IResult> GetUserByIdAsync([FromServices] IUserService userService, int id, CancellationToken cancellationToken = default)
        => (await userService.FindUserAsync(id, cancellationToken))
            .Match<IResult>(
                user => TypedResults.Ok(user),
                notFound => TypedResults.NotFound());

    internal static async Task<IResult> GetUsersAsync([FromServices] IUserService userService, CancellationToken cancellationToken) => TypedResults.Ok(await userService.ListUsersAsync(cancellationToken));

    internal static async Task<IResult> DeleteUserAsync([FromServices] IUserService userService, int id, CancellationToken cancellationToken)
        => (await userService.DeleteUserAsync(id, cancellationToken))
            .Match<IResult>(
                success => TypedResults.Ok(),
                notFound => TypedResults.NotFound(),
                error => TypedResults.BadRequest(error.Message));

    internal static async Task<IResult> UpdateUserAsync([FromServices] IUserService userService, int id, [FromBody]UpdateUserRequest request, CancellationToken cancellationToken)
    {   
        var updateUserCommand = UpdateUserCommand.CreateFrom(id, ValidEmailAddress.CreateFrom(request.Email), ValidPassword.CreateFrom(request.Password));

        if (updateUserCommand is null)
        {
            return TypedResults.BadRequest(UpdateUserCommand.ValidationRequirements);
        }

        (var success, var error) = await userService.UpdateUserAsync(updateUserCommand, cancellationToken);

        if (!success)
        {
            return TypedResults.BadRequest(error);
        }

        return TypedResults.Ok();
    }

    internal static async Task<IResult> CreateUserAsync([FromServices] IUserService userService, [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var validationProblems = new List<(string field, string description)>();

        var email = ValidEmailAddress.CreateFrom(request.Email);
        var password = ValidPassword.CreateFrom(request.Password);

        if (email is null)
        {
            validationProblems.Add((nameof(request.Email), ValidEmailAddress.ValidationRequirements));
        }
        if (password is null)
        {
            validationProblems.Add((nameof(request.Password), ValidPassword.ValidationRequirements));
        }

        if (validationProblems.Any())
        {
            var errors = validationProblems.ToDictionary(p => p.field, p => validationProblems.Where(vp => vp.field == p.field).Select(vp => vp.description).ToArray());
            return TypedResults.ValidationProblem(errors);
        }
        
        return (await userService.CreateUserAsync(new(email!, password!), cancellationToken))
            .Match<IResult>(
                success => TypedResults.CreatedAtRoute(nameof(GetUserByIdAsync), new { id = success.Value }),
                emailReserved => TypedResults.BadRequest(emailReserved.Message),
                userCreationFailed => TypedResults.BadRequest(userCreationFailed.Message));
    }
}