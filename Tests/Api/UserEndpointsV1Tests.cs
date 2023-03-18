using Api;
using ApplicationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Tests.Api;

internal class UserEndpointsV1Tests
{
    [Test]
    public void GivenGetUserIsCalled_WhenUserDoesNotExists_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.FindUser(default).ReturnsNull();
        
        var sut = () => UserEndpointsV1.GetUserById(userService, default);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Test]
    public void GivenGetUserIsCalled_WhenUserExists_ThenReturnCorrectly()
    {
        // Arrange        
        var user = new UserDTO(1, "a@b");
        var userService = Substitute.For<IUserService>();
        userService.FindUser(default).ReturnsForAnyArgs(user);

        var sut = () => UserEndpointsV1.GetUserById(userService, default);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok<UserDTO>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = user,
            });
    }

    [Test]
    public void GivenGetUsersIsCalled_WhenThereAreUsers_ThenReturnsCorrectly()
    {
        // Arrange
        var users = new List<UserDTO> { new(1, "a@b"), new(2, "b@c"), };
        var userService = Substitute.For<IUserService>();
        userService.ListUsers().Returns(users);

        var sut = () => UserEndpointsV1.GetUsers(userService);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Test]
    public void GivenGetUsersIsCalled_WhenTherAreNoUsers_ThenReturnsCorrectly()
    {
        // Arrange        
        var users = new List<UserDTO>();
        var userService = Substitute.For<IUserService>();
        userService.ListUsers().Returns(users);
        
        var sut = () => UserEndpointsV1.GetUsers(userService);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Test]
    public void GivenDeleteUserIsCalled_WhenDeleteFails_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUser(default).Returns(false);
        
        var sut = () => UserEndpointsV1.DeleteUser(userService, default);
        
        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = UserEndpointsV1.DeleteUserError,
            });
    }

    [Test]
    public void GivenDeleteUserIsCalled_WhenDeleteSucceeds_ThenReturnCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUser(default).Returns(true);
        
        var sut = () => UserEndpointsV1.DeleteUser(userService, default);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Test]
    public void GivenUpdateUserIsCalled_WhenUpdateFails_ThenReturnCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(default, default);
        var errorMessage = "Houston we have a problem.";
        var userService = Substitute.For<IUserService>();
        userService.UpdateUser(default, default, default).ReturnsForAnyArgs((false, errorMessage));
        
        var sut = () => UserEndpointsV1.UpdateUser(userService, default, updateUserRequest);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = errorMessage,
            });
    }

    [Test]
    public void GivenUpdateUserIsCalled_WhenUpdateSucceeds_ThenReturnCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(default, default);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUser(default, default, default).ReturnsForAnyArgs((true, default));
        
        var sut = () => UserEndpointsV1.UpdateUser(userService, default, updateUserRequest);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Test]
    public void GivenCreateUserIsCalled_WhenCreationFails_ThenReturnCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(string.Empty, string.Empty);
        var validationErrorMessages = new Dictionary<string, string[]>() { {"Houston", new string[] { "we have a problem." } }};
        var userService = Substitute.For<IUserService>();
        userService.CreateUser(default, default).ReturnsForAnyArgs((false, default, validationErrorMessages));
        
        var sut = () => UserEndpointsV1.CreateUser(userService, createUserRequest);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(validationErrorMessages)
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                },
            });
    }

    [Test]
    public void GivenCreateUserIsCalled_WhenCreationSucceeds_ThenReturnCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest(string.Empty, string.Empty);
        var id = 10;
        var userService = Substitute.For<IUserService>();
        userService.CreateUser(default, default).ReturnsForAnyArgs((true, id, null));
        
        var sut = () => UserEndpointsV1.CreateUser(userService, createUserRequest);

        // Act
        var result = sut.Invoke();

        // Assert
        result.Should().BeOfType<CreatedAtRoute>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 201,
                RouteName = nameof(UserEndpointsV1.GetUserById),
                RouteValues = new RouteValueDictionary { { nameof(id), id }, },
            });
    }
}
