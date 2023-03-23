using ApplicationServices;
using Domain;
using OneOf;
using OneOf.Types;

namespace Tests.ApplicationServices;

internal class UserServiceTests
{
    [Test]
    public async Task GivenCreateUserIsCalled_WhenEmailIsReserved_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com")!;
        var password = ValidPassword.CreateFrom("password123")!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012")!;
        var userFromRepo = new User(1, email, HashedPassword.CreateFrom(password, passwordSalt));
        userRepository.FindUserByEmailAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<User?>(userFromRepo));
        var command = new CreateUserCommand(email, password);

        var sut = new UserService(userRepository);

        // Act
        (await sut.CreateUserAsync(command, default)).TryPickT1(out var result, out var _);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmailReservedError>();
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenPersistingUserFails_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com")!;
        var password = ValidPassword.CreateFrom("password123")!;
        userRepository.FindUserByEmailAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<User?>(null));
        userRepository.CreateUserAsync(default!, default!, default).ReturnsForAnyArgs(
            Task.FromResult<OneOf<Success<int>, UserCreationFailedError>>(new UserCreationFailedError()));
        var command = new CreateUserCommand(email, password);

        var sut = new UserService(userRepository);

        // Act
        (await sut.CreateUserAsync(command, default)).TryPickT2(out var result, out var _);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UserCreationFailedError>();
    }

    [Test]
    public async Task GivenCreateUserIsCalled_WhenUserIsCreatedSuccessfully_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com")!;
        var password = ValidPassword.CreateFrom("password123")!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012")!;
        var userPersisted = new User(1, email, HashedPassword.CreateFrom(password, passwordSalt));
        userRepository.FindUserByEmailAsync(default!, default).ReturnsForAnyArgs(Task.FromResult<User?>(null));
        userRepository.CreateUserAsync(default!, default!, default).ReturnsForAnyArgs(
            Task.FromResult<OneOf<Success<int>, UserCreationFailedError>>(new Success<int>(userPersisted.Id)));
        var command = new CreateUserCommand(email, password);

        var sut = new UserService(userRepository);

        // Act
        (await sut.CreateUserAsync(command, default)).TryPickT0(out var result, out var _);

        // Assert
        result.Should().BeOfType<Success<int>>().Which.Value.Should().Be(1);
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenUserIsNotFound_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.DeleteUserAsync(default, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, NotFound, UserDeletionFailedError>>(new NotFound()));

        var sut = new UserService(userRepository);

        // Act
        var result = (await sut.DeleteUserAsync(1, default)).Match<NotFound?>(x => null, y => y, t => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NotFound>();
    }
}
