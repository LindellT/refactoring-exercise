using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests;

public class UserRepositoryTests
{
    [Fact]
    public async Task GivenCreateUserAsyncIsCalled_WhenCreatedSuccessfully_ThenReturnsCorrectly()
    {
        // Arrange
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com").Match<ValidEmailAddress?>(validEmailAddress => validEmailAddress, emailValidationError => null)!;
        var password = ValidPassword.CreateFrom("password123").Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null)!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012").Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null)!;
        var passwordHash = HashedPassword.CreateFrom(password, passwordSalt)!;
        using SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();
        var contextOptions = new DbContextOptionsBuilder<UserContext>()
            .UseSqlite(connection)
            .Options;
        using UserContext context = new(contextOptions);
        await context.Database.MigrateAsync();
        var sut = new UserRepository(context);

        // Act
        var result = (await sut.CreateUserAsync(email, passwordHash, CancellationToken.None)).Match<Success<int>?>(success => success, userCreationFailedError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Success<int>>().Which.Value.Should().Be(1);
    }

    [Fact]
    public async Task GivenCreateUserAsyncIsCalled_WhenCreationFails_ThenReturnsCorrectly()
    {
        // Arrange
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com").Match<ValidEmailAddress?>(validEmailAddress => validEmailAddress, emailValidationError => null)!;
        var password = ValidPassword.CreateFrom("password123").Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null)!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012").Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null)!;
        var passwordHash = HashedPassword.CreateFrom(password, passwordSalt)!;
        using SqliteConnection connection = new("Filename=:memory:");
        var contextOptions = new DbContextOptionsBuilder<UserContext>()
            .UseSqlite(connection)
            .Options;
        using var context = Substitute.ForPartsOf<UserContext>(contextOptions);
        context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(0);
        var sut = new UserRepository(context);

        // Act
        var result = (await sut.CreateUserAsync(email, passwordHash, CancellationToken.None)).Match<UserCreationFailedError?>(success => null, userCreationFailedError => userCreationFailedError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UserCreationFailedError>();
    }

    [Fact]
    public async Task GivenDeleteUserAsyncIsCalled_WhenDeleted_ThenReturnsCorrectly()
    {
        // Arrange
        var email = ValidEmailAddress.CreateFrom("bill@microsoft.com").Match<ValidEmailAddress?>(validEmailAddress => validEmailAddress, emailValidationError => null)!;
        var password = ValidPassword.CreateFrom("password123").Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null)!;
        var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012").Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null)!;
        var passwordHash = HashedPassword.CreateFrom(password, passwordSalt)!;
        using SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();
        var contextOptions = new DbContextOptionsBuilder<UserContext>()
            .UseSqlite(connection)
            .Options;
        using UserContext context = new(contextOptions);
        await context.Database.MigrateAsync();
        var sut = new UserRepository(context);
        var userId = (await sut.CreateUserAsync(email, passwordHash, CancellationToken.None)).Match<Success<int>?>(success => success, userCreationFailedError => null)!.Value.Value;

        // Act
        var result = (await sut.DeleteUserAsync(userId, CancellationToken.None)).Match<Success?>(success => success, notFound => null, userCreationFailedError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<Success>();
    }

    [Fact]
    public async Task GivenDeleteUserAsyncIsCalled_WhenUserNotFound_ThenReturnsCorrectly()
    {
		// Arrange
		var email = ValidEmailAddress.CreateFrom("bill@microsoft.com").Match<ValidEmailAddress?>(validEmailAddress => validEmailAddress, emailValidationError => null)!;
		var password = ValidPassword.CreateFrom("password123").Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null)!;
		var passwordSalt = ValidPasswordSalt.CreateFrom("12345678901235467890123456789012").Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null)!;
		var passwordHash = HashedPassword.CreateFrom(password, passwordSalt)!;
		using SqliteConnection connection = new("Filename=:memory:");
		await connection.OpenAsync();
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseSqlite(connection)
			.Options;
		using UserContext context = new(contextOptions);
		await context.Database.MigrateAsync();
		var sut = new UserRepository(context);

		// Act
		var result = (await sut.DeleteUserAsync(1, CancellationToken.None)).Match<NotFound?>(success => null, notFound => notFound, userCreationFailedError => null);

		// Assert
		result.Should().NotBeNull().And.BeOfType<NotFound>();
	}

    [Fact]
    public void GivenDeleteUserAsyncIsCalled_WhenDeleteFails_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserAsyncIsCalled_WhenFound_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserAsyncIsCalled_WhenNotFound_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserAsyncIsCalled_WhenSoftDeleted_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserByEmailAsyncIsCalled_WhenFound_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserByEmailAsyncIsCalled_WhenNotFound_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenFindUserByEmailAsyncIsCalled_WhenSoftDeleted_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenListUsersAsyncIsCalled_WhenUsersExist_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenListUsersAsyncIsCalled_WhenUsersDontExist_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenListUsersAsyncIsCalled_WhenSoftDeleted_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenUpdateUserAsyncIsCalled_WhenUpdated_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenUpdateUserAsyncIsCalled_WhenUserNotFound_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public void GivenUpdateUserAsyncIsCalled_WhenUpdateFails_ThenReturnsCorrectly()
    {
        // Arrange

        // Act

        // Assert
        throw new NotImplementedException();
    }
}