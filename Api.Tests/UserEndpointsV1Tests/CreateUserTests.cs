using Microsoft.AspNetCore.Routing;

namespace Api.Tests.UserEndpointsV1Tests;

public sealed class CreateUserTests
{
    private const string ValidationProblemType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    internal static readonly string[] EmailValidationErrorMessage = ["Invalid email. Email must have a recipient and domain and contain @ sign.",];
    internal static readonly string[] PasswordValidationErrorMessage = ["Invalid password. Password length must be at least 8 characters.",];

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