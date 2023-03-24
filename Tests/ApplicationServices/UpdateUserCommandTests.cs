using ApplicationServices;
using Domain;

namespace Tests.ApplicationServices;

internal sealed class UpdateUserCommandTests
{
    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsNull()
    {
        // Arrange
        var userId = 1;
        var email = ValidEmailAddress.CreateFrom(null);
        var password = ValidPassword.CreateFrom(null);

        // Act
        var result = UpdateUserCommand.CreateFrom(userId, email, password);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    [TestCase("bill@microsoft.com", "password123")]
    [TestCase("bill@microsoft.com", null)]
    [TestCase(null, "password123")]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly(string? email, string? password)
    {
        // Arrange
        var userId = 1;
        var validEmail = ValidEmailAddress.CreateFrom(email);
        var validPassword = ValidPassword.CreateFrom(password);

        // Act
        var result = UpdateUserCommand.CreateFrom(userId, validEmail, validPassword);

        // Assert
        result.Should().NotBeNull().And.BeOfType<UpdateUserCommand>().Which.Should().BeEquivalentTo(
            new 
            {
                Id = userId,
                EmailAddress = validEmail,
                Password = validPassword,
            });
    }
}