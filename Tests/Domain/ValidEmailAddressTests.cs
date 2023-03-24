using Domain;

namespace Tests.Domain;

internal sealed class ValidEmailAddressTests
{
    [Test]
    [TestCase("bill£microsoft.com")]
    [TestCase("@microsoft.com")]
    [TestCase("bill@")]
    [TestCase("bill@ ")]
    [TestCase(" @microsoft.com")]
    [TestCase(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsCorrectly(string? email)
    {
        // Arrange

        // Act
        var result = ValidEmailAddress.CreateFrom(email).Match<EmailValidationError?>(validEmailAddress => null, emailValidationError => emailValidationError);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmailValidationError>();
    }

    [Test]
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