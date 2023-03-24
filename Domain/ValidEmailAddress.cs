using OneOf;

namespace Domain;

public sealed record ValidEmailAddress
{
#pragma warning disable CS0628 // New protected member declared in sealed type, used by EF Core
    protected ValidEmailAddress() { }
#pragma warning restore CS0628 // New protected member declared in sealed type

    private ValidEmailAddress(string address)
    {
        Address = address;
    }

    public string Address { get; init; } = null!;

    public static OneOf<ValidEmailAddress, EmailValidationError> CreateFrom(string? address)
    {
        if (address is null)
        {
            return new EmailValidationError();
        }

        var addressParts = address.Split('@');
        if (addressParts.Length < 2 || addressParts[0].Trim().Length == 0 || addressParts[1].Trim().Length == 0)
        { 
            return new EmailValidationError();
        }

        return new ValidEmailAddress(address);
    }
}