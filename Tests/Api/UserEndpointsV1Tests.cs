using Api;
using ApplicationServices;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using OneOf;
using OneOf.Types;

namespace Tests.Api;

internal class UserEndpointsV1Tests
{
    [Test]
    public async Task GivenGetUserIsCalled_WhenUserDoesNotExists_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.FindUserAsync(default, default).ReturnsNull();

        var sut = () => UserEndpointsV1.GetUserByIdAsync(userService, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Test]
    public async Task GivenGetUserIsCalled_WhenUserExists_ThenReturnCorrectly()
    {
        // Arrange        
        var user = new UserDTO(1, "a@b");
        var userService = Substitute.For<IUserService>();
        userService.FindUserAsync(default, default).ReturnsForAnyArgs(user);

        var sut = () => UserEndpointsV1.GetUserByIdAsync(userService, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Ok<UserDTO>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = user,
            });
    }

    [Test]
    public async Task GivenGetUsersIsCalled_WhenThereAreUsers_ThenReturnsCorrectly()
    {
        // Arrange
        var users = new List<UserDTO> { new(1, "a@b"), new(2, "b@c"), };
        var userService = Substitute.For<IUserService>();
        userService.ListUsersAsync(default).Returns(users);

        var sut = () => UserEndpointsV1.GetUsersAsync(userService, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Test]
    public async Task GivenGetUsersIsCalled_WhenTherAreNoUsers_ThenReturnsCorrectly()
    {
        // Arrange        
        var users = new List<UserDTO>();
        var userService = Substitute.For<IUserService>();
        userService.ListUsersAsync(default).Returns(users);

        var sut = () => UserEndpointsV1.GetUsersAsync(userService, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenDeleteFails_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new UserDeletionFailedError()));

        var sut = () => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new UserDeletionFailedError().Message,
            });
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenUserIsNotFound_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new OneOf.Types.NotFound()));

        var sut = () => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenDeleteSucceeds_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new Success()));

        var sut = () => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task GivenUpdateUserIsCalled_WhenUpdateFails_ThenReturnCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest("bill@microsoft.com", default);
        var errorMessage = "Houston we have a problem.";
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs((false, errorMessage));

        var sut = () => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = errorMessage,
            });
    }

    [Test]
    public async Task GivenUpdateUserIsCalled_WhenParametersAreInvalid_ThenReturnCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(null, null);
        var errorMessage = "Houston we have a problem.";
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs((false, errorMessage));

        var sut = () => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = UpdateUserCommand.ValidationRequirements,
            });
    }

    [Test]
    [TestCase("bill@microsoft.com", "password123")]
    [TestCase("bill@microsoft.com", null)]
    [TestCase(null, "password123")]
    public async Task GivenUpdateUserIsCalled_WhenParametersAreValid_ThenReturnCorrectly(string? email, string? password)
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(email, password);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs((true, default));

        var sut = () => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenCreationFails_ThenReturnCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new UserCreationFailedError());

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new UserCreationFailedError().Message,
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenEmailIsReserved_ThenReturnCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new EmailReservedError());

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new EmailReservedError().Message,
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenCreationSucceeds_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var id = 10;
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new Success<int>(id));

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<CreatedAtRoute>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 201,
                RouteName = nameof(UserEndpointsV1.GetUserByIdAsync),
                RouteValues = new RouteValueDictionary { { nameof(id), id }, },
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenEmailAndPasswordAreInvalid_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest(null!, null!);
        var userService = Substitute.For<IUserService>();

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Email), new string[] { ValidEmailAddress.ValidationRequirements } },
                        { nameof(createUserRequest.Password), new string[] { ValidPassword.ValidationRequirements } },
                    })
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                },
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenPasswordIsInvalid_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", null!);
        var userService = Substitute.For<IUserService>();

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Password), new string[] { ValidPassword.ValidationRequirements } },
                    })
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                },
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenEmailIsInvalid_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest(null!, "password123");
        var userService = Substitute.For<IUserService>();

        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result?.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Email), new string[] { ValidEmailAddress.ValidationRequirements } },
                    })
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                },
            });
    }
}