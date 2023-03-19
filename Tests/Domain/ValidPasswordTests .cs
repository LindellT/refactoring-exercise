using Domain;

namespace Tests.Domain;

internal class ValidPasswordTests
{
    [Test]    
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsNull()
    {
        // Arrange
        var password = "1234567";

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
