namespace Api.Tests.UserEndpointsV1Tests;

public sealed class GetUserTests
{
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
}
