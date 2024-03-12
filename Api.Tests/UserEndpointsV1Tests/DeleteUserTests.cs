namespace Api.Tests.UserEndpointsV1Tests;

public sealed class DeleteUserTests
{
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
}
