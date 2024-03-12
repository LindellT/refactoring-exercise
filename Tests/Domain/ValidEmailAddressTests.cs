namespace Tests.Domain;

public sealed class ValidEmailAddressTests
{
    [Theory]
    [InlineData("bill£microsoft.com")]
    [InlineData("@microsoft.com")]
    [InlineData("bill@")]
    [InlineData("bill@ ")]
    [InlineData(" @microsoft.com")]
    [InlineData(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsCorrectly(string? email)
    {
        // Arrange

        // Act
        var result = ValidEmailAddress.CreateFrom(email).Match<EmailValidationError?>(validEmailAddress => null, emailValidationError => emailValidationError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmailValidationError>();
    }

    [Fact]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var email = "bill@microsoft.com";

        // Act
        var result = ValidEmailAddress.CreateFrom(email).Match<ValidEmailAddress?>(validEmailAddress => validEmailAddress, emailValidationError => null);

        // Assert
        result.Should().NotBeNull().And.BeOfType<ValidEmailAddress>().Which.Address.Should().Be(email);
    }
}