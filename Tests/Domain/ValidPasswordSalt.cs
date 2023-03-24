using Domain;

namespace Tests.Domain;

internal sealed class ValidPasswordSaltTests
{
    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsNull()
    {
        // Arrange
        var salt = "1234567890123456789012345678901";

        // Act
        var result = ValidPasswordSalt.CreateFrom(salt);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var salt = "12345678901234567890123456789012";

        // Act
        var result = ValidPasswordSalt.CreateFrom(salt);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidPasswordSalt>().Which.Salt.Should().Be(salt);
    }
}