using Domain;

namespace Tests.Domain;

internal sealed class ValidPasswordTests
{
    [Test]
    [TestCase("1234567")]
    [TestCase(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsCorrectly(string? password)
    {
        // Arrange

        // Act
        var result = ValidPassword.CreateFrom(password).Match<PasswordValidationError?>(validPassword => null, passwordValidationError => passwordValidationError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<PasswordValidationError>();
    }

    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var password = "12345678";

        // Act
        var result = ValidPassword.CreateFrom(password).Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidPassword>().Which.Password.Should().Be(password);
    }
}