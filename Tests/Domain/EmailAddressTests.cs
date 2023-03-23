using Domain;

namespace Tests.Domain;

internal class EmailAddressTests
{
    [Test]
    [TestCase("bill£microsoft.com")]
    [TestCase("@microsoft.com")]
    [TestCase("bill@")]
    [TestCase("bill@ ")]
    [TestCase(" @microsoft.com")]
    [TestCase(null)]
    public void GivenSmartConstructerIsCalled_WhenParametersAreNotValid_ThenReturnsNull(string? email)
    {
        // Arrange

        // Act
        var result = ValidEmailAddress.CreateFrom(email);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsCorrectly()
    {
        // Arrange
        var email = "bill@microsoft.com";

        // Act
        var result = ValidEmailAddress.CreateFrom(email);

        // Assert
        result.Should().BeOfType<ValidEmailAddress>().Which.Address.Should().Be(email);
    }
}