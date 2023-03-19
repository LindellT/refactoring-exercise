using ApplicationServices;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api;

internal static class UserEndpointsV1
{
    internal const string DeleteUserError = "Couldn't delete user.";

    public static RouteGroupBuilder MapUsersApiV1(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateUser)
            .WithName(nameof(CreateUser))
            .WithOpenApi();

        group.MapPut("/{id:int}", UpdateUser)
            .WithName(nameof(UpdateUser))
            .WithOpenApi();

        group.MapDelete("/{id:int}", DeleteUser)
            .WithName(nameof(DeleteUser))
            .WithOpenApi();

        group.MapGet("/", GetUsers)
            .WithName(nameof(GetUsers))
            .WithOpenApi();

        group.MapGet("/{id:int}", GetUserById)
            .WithName(nameof(GetUserById))
            .WithOpenApi();

        return group;
    }

    internal static IResult GetUserById([FromServices] IUserService userService, int id)
    {
        var user = userService.FindUser(id);

        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    internal static IResult GetUsers([FromServices] IUserService userService) => TypedResults.Ok(userService.ListUsers());

    internal static IResult DeleteUser([FromServices] IUserService userService, int id) => userService.DeleteUser(id) ? TypedResults.Ok() : TypedResults.BadRequest(UserEndpointsV1.DeleteUserError);

    internal static IResult UpdateUser([FromServices] IUserService userService, int id, [FromBody]UpdateUserRequest request)
    {   
        (var success, var error) = userService.UpdateUser(
            id,
            request.Email is null ? null : ValidEmailAddress.CreateFrom(request.Email),
            request.Password is null ? null : ValidPassword.CreateFrom(request.Password));

        if (!success)
        {
            return TypedResults.BadRequest(error);
        }

        return TypedResults.Ok();
    }

    internal static IResult CreateUser([FromServices] IUserService userService, [FromBody] CreateUserRequest request)
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

        if (email is not null && password is not null)
        {
            (var success, var id, var error) = userService.CreateUser(email, password);

            if (!success && error is not null)
            {
                validationProblems.Add((string.Empty, error));
                
            }
            else
            {
                return TypedResults.CreatedAtRoute(nameof(GetUserById), new { id });
            }
        }

        var errors = validationProblems.ToDictionary(p => p.field, p => validationProblems.Where(vp => vp.field == p.field).Select(vp => vp.description).ToArray());
        return TypedResults.ValidationProblem(errors);
    }
}