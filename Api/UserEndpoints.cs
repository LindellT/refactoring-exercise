using ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace Api;

internal static class UserEndpointsV1
{
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

    static IResult GetUserById([FromServices] IUserService userService, int id)
    {
        var user = userService.FindUser(id);

        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    static IResult GetUsers([FromServices] IUserService userService) => TypedResults.Ok(userService.ListUsers());

    static IResult DeleteUser([FromServices] IUserService userService, int id) => userService.DeleteUser(id) ? TypedResults.Ok() : TypedResults.BadRequest("Couldn't delete user.");

    static IResult UpdateUser([FromServices] IUserService userService, int id, [FromBody]UpdateUserRequest request)
    {
        (var success, var error) = userService.UpdateUser(id, request.Email, request.Password);

        if (!success)
        {
            return TypedResults.BadRequest(error);
        }

        return TypedResults.Ok();
    }

    static IResult CreateUser([FromServices] IUserService userService, [FromBody] CreateUserRequest request)
    {
        (var success, var id, var errors) = userService.CreateUser(request.Email, request.Password);

        if (!success)
        {
            return TypedResults.ValidationProblem(errors!);
        }

        return TypedResults.CreatedAtRoute(nameof(GetUserById), new { id });
    }
}