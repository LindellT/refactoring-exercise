using Domain;

namespace Tests.Domain;

internal sealed class HashedPasswordTests
{
    [Test]
    public void GivenSmartConstructerIsCalled_WhenParametersAreValid_ThenReturnsHash()
    {
        // Arrange
        var password = "12354678";
        var salt = "12345678901234567890123456789012";
        var validPasswordSalt = ValidPasswordSalt.CreateFrom(salt).Match<ValidPasswordSalt?>(validPasswordSalt => validPasswordSalt, passwordSaltValidationError => null)!;
        var validPassword = ValidPassword.CreateFrom(password).Match<ValidPassword?>(validPassword => validPassword, passwordValidationError => null)!;

        // Act
        var result = HashedPassword.CreateFrom(validPassword, validPasswordSalt);

        // Assert
        result.Should().NotBeNull().And.BeOfType<HashedPassword>().Which.Hash.Should().NotContain(password).And.HaveLength(64);
    }
}