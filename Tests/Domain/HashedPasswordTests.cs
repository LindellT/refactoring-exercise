using Domain;

namespace Tests.Domain;

internal class HashedPasswordTests
{
    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsHash()
    {
        // Arrange
        var password = "12354678";
        var salt = "12345678901234567890123456789012";

        // Act
        var result = HashedPassword.CreateFrom(ValidPassword.CreateFrom(password)!, ValidPasswordSalt.CreateFrom(salt)!);

        // Assert
        result.Should().NotBeNull().And.BeOfType<HashedPassword>().Which.Hash.Should().NotContain(password).And.HaveLength(64);
    }
}