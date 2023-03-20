using Domain;

namespace Tests.Domain;

internal class ValidPasswordTests
{
    [Test]
    [TestCase("1234567")]
    [TestCase(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsNull(string? password)
    {
        // Arrange

        // Act
        var result = ValidPassword.CreateFrom(password);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var password = "12345678";

        // Act
        var result = ValidPassword.CreateFrom(password);

        // Assert
        result.Should().BeOfType<ValidPassword>().Which.Password.Should().Be(password);
    }
}