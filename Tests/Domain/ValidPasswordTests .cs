namespace Tests.Domain;

public sealed class ValidPasswordTests
{
    [Theory]
    [InlineData("1234567")]
    [InlineData(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsCorrectly(string? password)
    {
        // Arrange

        // Act
        var result = ValidPassword.CreateFrom(password).Match<PasswordValidationError?>(validPassword => null, passwordValidationError => passwordValidationError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<PasswordValidationError>();
    }

    [Fact]
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