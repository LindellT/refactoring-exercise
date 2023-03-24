using Domain;

namespace Tests.Domain;

internal sealed class ValidPasswordSaltTests
{
    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsCorrectly()
    {
        // Arrange
        var salt = "1234567890123456789012345678901";

        // Act
        var result = ValidPasswordSalt.CreateFrom(salt).Match<PasswordSaltValidationError?>(validPasswordSalt => null, passwordSaltValidationError => passwordSaltValidationError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<PasswordSaltValidationError>();
    }

    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var salt = "12345678901234567890123456789012";

        // Act
        var result = ValidPasswordSalt.CreateFrom(salt).Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidPasswordSalt>().Which.Salt.Should().Be(salt);
    }
}