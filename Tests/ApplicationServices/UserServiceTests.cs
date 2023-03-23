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
        var result = (await sut.CreateUserAsync(command, default)).Match<EmailReservedError?>(success => null, emailReservedError => emailReservedError, userCreationFailedError => null);

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
        var result = (await sut.CreateUserAsync(command, default)).Match<UserCreationFailedError?>(success => null, emailReservedError => null, userCreationFailedError => userCreationFailedError);

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
        var result = (await sut.CreateUserAsync(command, default)).Match<Success<int>?>(success => success, emailReservedError => null, userCreationFailedError => null);

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
        var result = (await sut.DeleteUserAsync(1, default)).Match<NotFound?>(success => null, notFound => notFound, userDeletionFailedError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NotFound>();
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenUserDeletionFails_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.DeleteUserAsync(default, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, NotFound, UserDeletionFailedError>>(new UserDeletionFailedError()));

        var sut = new UserService(userRepository);

        // Act
        var result = (await sut.DeleteUserAsync(1, default)).Match<UserDeletionFailedError?>(success => null, notFound => null, userDeletionFailedError => userDeletionFailedError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UserDeletionFailedError>();
    }

    [Test]
    public async Task GivenDeleteUserIsCalled_WhenUserDeletionSucceeds_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.DeleteUserAsync(default, default).ReturnsForAnyArgs(Task.FromResult<OneOf<Success, NotFound, UserDeletionFailedError>>(new Success()));

        var sut = new UserService(userRepository);

        // Act
        var result = (await sut.DeleteUserAsync(1, default)).Match<Success?>(success => success, notFound => null, userDeletionFailedError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Success>();
    }

    [Test]
    public async Task GivenFindUserIsCalled_WhenUserIsNotFound_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        userRepository.FindUserAsync(default, default).ReturnsForAnyArgs(Task.FromResult<OneOf<User, NotFound>>(new NotFound()));

        var sut = new UserService(userRepository);

        // Act
        var result = (await sut.FindUserAsync(1, default)).Match<NotFound?>(userDto => null, notFound => notFound);

        // Assert
        result.Should().NotBeNull().And.BeOfType<NotFound>();
    }

    [Test]
    public async Task GivenFindUserIsCalled_WhenUserIsFound_ThenReturnsCorrectly()
    {
        // Arrange
        var userRepository = Substitute.For<IUserRepository>();
        var emailAddress = "bill@microsoft.com";
        var email = ValidEmailAddress.CreateFrom(emailAddress)!;
        var password = ValidPassword.CreateFrom("password123")!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012")!;
        var user = new User(1, email, HashedPassword.CreateFrom(password, passwordSalt));
        userRepository.FindUserAsync(default, default).ReturnsForAnyArgs(Task.FromResult<OneOf<User, NotFound>>(user));

        var sut = new UserService(userRepository);

        // Act
        var result = (await sut.FindUserAsync(1, default)).Match<UserDTO?>(userDto => userDto, notFound => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UserDTO>().And.BeEquivalentTo(new UserDTO(1, emailAddress));
    }
}