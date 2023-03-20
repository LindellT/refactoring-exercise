using Api;
using ApplicationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

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
        result.Should().BeOfType<NotFound>().Which.StatusCode.Should().Be(404);
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
        result.Should().BeOfType<Ok<UserDTO>>().Which.Should().BeEquivalentTo(
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
        result.Should().BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
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
        result.Should().BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
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
        userService.DeleteUserAsync(default, default).Returns(false);
        
        var sut = () => UserEndpointsV1.DeleteUserAsync(userService, default, default);
        
        // Act
        var result = await sut.Invoke();

        // Assert
        result.Should().BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = UserEndpointsV1.DeleteUserError,
            });
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenDeleteSucceeds_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(true);
        
        var sut = () => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok>().Which.StatusCode.Should().Be(200);
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
        result.Should().BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
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
        result.Should().BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
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
        result.Should().BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenCreationFails_ThenReturnCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var validationErrorMessage = "Houston, we have a problem.";
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs((false, default, validationErrorMessage));
        
        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result.Should().BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(new Dictionary<string, string[]> { { string.Empty, new string[] { validationErrorMessage } } })
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                },
            });
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenCreationSucceeds_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var id = 10;
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs((true, id, null));
        
        var sut = () => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut.Invoke();

        // Assert
        result.Should().BeOfType<CreatedAtRoute>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 201,
                RouteName = nameof(UserEndpointsV1.GetUserByIdAsync),
                RouteValues = new RouteValueDictionary { { nameof(id), id }, },
            });
    }
}