namespace Api.Tests.UserEndpointsV1Tests;

public sealed class UpdateUserTests
{
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
}
