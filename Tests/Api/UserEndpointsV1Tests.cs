using Api;
using ApplicationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Tests.Api;

public sealed class UserEndpointsV1Tests
{
    private const string ValidationProblemType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    internal static readonly string[] EmailValidationErrorMessage = ["Invalid email. Email must have a recipient and domain and contain @ sign.",];
    internal static readonly string[] PasswordValidationErrorMessage = ["Invalid password. Password length must be at least 8 characters.",];

    [Fact]
    public async Task GivenGetUserIsCalled_WhenUserDoesNotExists_ThenReturnsCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.FindUserAsync(default, default).Returns(Task.FromResult<OneOf<UserDTO, OneOf.Types.NotFound>>(new OneOf.Types.NotFound()));

        Task<IResult> sut() => UserEndpointsV1.GetUserByIdAsync(userService, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GivenGetUserIsCalled_WhenUserExists_ThenReturnsCorrectly()
    {
        // Arrange        
        var user = new UserDTO(1, "a@b");
        var userService = Substitute.For<IUserService>();
        userService.FindUserAsync(default, default).ReturnsForAnyArgs(user);

        Task<IResult> sut() => UserEndpointsV1.GetUserByIdAsync(userService, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Ok<UserDTO>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = user,
            });
    }

    [Fact]
    public async Task GivenGetUsersIsCalled_WhenThereAreUsers_ThenReturnsCorrectly()
    {
        // Arrange
        var users = new List<UserDTO> { new(1, "a@b"), new(2, "b@c"), };
        var userService = Substitute.For<IUserService>();
        userService.ListUsersAsync(default).Returns(users);

        Task<IResult> sut() => UserEndpointsV1.GetUsersAsync(userService, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Fact]
    public async Task GivenGetUsersIsCalled_WhenTherAreNoUsers_ThenReturnsCorrectly()
    {
        // Arrange        
        var users = new List<UserDTO>();
        var userService = Substitute.For<IUserService>();
        userService.ListUsersAsync(default).Returns(users);

        Task<IResult> sut() => UserEndpointsV1.GetUsersAsync(userService, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Ok<List<UserDTO>>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 200,
                Value = users,
            });
    }

    [Fact]
    public async Task GivenDeleteUserIsCalled_WhenDeleteFails_ThenReturnsCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new UserDeletionFailedError()));

        Task<IResult> sut() => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new UserDeletionFailedError().Message,
            });
    }

    [Fact]
    public async Task GivenDeleteUserIsCalled_WhenUserIsNotFound_ThenReturnsCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new OneOf.Types.NotFound()));

        Task<IResult> sut() => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GivenDeleteUserIsCalled_WhenDeleteSucceeds_ThenReturnsCorrectly()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();
        userService.DeleteUserAsync(default, default).Returns(
            Task.FromResult<OneOf<Success, OneOf.Types.NotFound, UserDeletionFailedError>>(new Success()));

        Task<IResult> sut() => UserEndpointsV1.DeleteUserAsync(userService, default, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GivenUpdateUserIsCalled_WhenUpdateFails_ThenReturnsCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest("bill@microsoft.com", default);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, OneOf.Types.NotFound, EmailReservedError, UserUpdateFailedError>>(new UserUpdateFailedError()));

        Task<IResult> sut() => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = "Updating user failed.",
            });
    }

    [Fact]
    public async Task GivenUpdateUserIsCalled_WhenEmailIsReserved_ThenReturnsCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest("bill@microsoft.com", default);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, OneOf.Types.NotFound, EmailReservedError, UserUpdateFailedError>>(new EmailReservedError()));

        Task<IResult> sut() => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = "Email reserved.",
            });
    }

    [Fact]
    public async Task GivenUpdateUserIsCalled_WhenUserIsNotFound_ThenReturnsCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest("bill@microsoft.com", default);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, OneOf.Types.NotFound, EmailReservedError, UserUpdateFailedError>>(new OneOf.Types.NotFound()));

        Task<IResult> sut() => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound>().Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GivenUpdateUserIsCalled_WhenParametersAreInvalid_ThenReturnsCorrectly()
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(null, null);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, OneOf.Types.NotFound, EmailReservedError, UserUpdateFailedError>>(new UserUpdateFailedError()));

        Task<IResult> sut() => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = "Either email or password has to be valid.",
            });
    }

    [Theory]
    [InlineData("bill@microsoft.com", "password123")]
    [InlineData("bill@microsoft.com", null)]
    [InlineData(null, "password123")]
    public async Task GivenUpdateUserIsCalled_WhenParametersAreValid_ThenReturnsCorrectly(string? email, string? password)
    {
        // Arrange        
        var updateUserRequest = new UpdateUserRequest(email, password);
        var userService = Substitute.For<IUserService>();
        userService.UpdateUserAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, OneOf.Types.NotFound, EmailReservedError, UserUpdateFailedError>>(new Success()));

        Task<IResult> sut() => UserEndpointsV1.UpdateUserAsync(userService, default, updateUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<Ok>().Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenCreationFails_ThenReturnsCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new UserCreationFailedError());

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new UserCreationFailedError().Message,
            });
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenEmailIsReserved_ThenReturnsCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new EmailReservedError());

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<BadRequest<string>>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                Value = new EmailReservedError().Message,
            });
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenCreationSucceeds_ThenReturnsCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", "password123");
        var id = 10;
        var userService = Substitute.For<IUserService>();
        userService.CreateUserAsync(default!, default).ReturnsForAnyArgs(new Success<int>(id));

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<CreatedAtRoute>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 201,
                RouteName = nameof(UserEndpointsV1.GetUserByIdAsync),
                RouteValues = new RouteValueDictionary { { nameof(id), id }, },
            });
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenEmailAndPasswordAreInvalid_ThenReturnsCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest(null!, null!);
        var userService = Substitute.For<IUserService>();

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Email), EmailValidationErrorMessage },
                        { nameof(createUserRequest.Password), PasswordValidationErrorMessage },
                    })
                {
                    Status = 400,
                    Type = ValidationProblemType,
                },
            });
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenPasswordIsInvalid_ThenReturnsCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest("bill@microsoft.com", null!);
        var userService = Substitute.For<IUserService>();

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Password), PasswordValidationErrorMessage },
                    })
                {
                    Status = 400,
                    Type = ValidationProblemType,
                },
            });
    }

    [Fact]
    public async Task GivenCreateUserIsCalled_WhenEmailIsInvalid_ThenReturnsCorrectly()
    {
        // Arrange        
        var createUserRequest = new CreateUserRequest(null!, "password123");
        var userService = Substitute.For<IUserService>();

        Task<IResult> sut() => UserEndpointsV1.CreateUserAsync(userService, createUserRequest, default);

        // Act
        var result = await sut();

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidationProblem>().Which.Should().BeEquivalentTo(
            new
            {
                StatusCode = 400,
                ProblemDetails = new HttpValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { nameof(createUserRequest.Email), EmailValidationErrorMessage },
                    })
                {
                    Status = 400,
                    Type = ValidationProblemType,
                },
            });
    }
}