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

    public const string ValidationRequirements = "Invalid email. Email must have a recipient and domain and contain @ sign.";

    public static ValidEmailAddress? CreateFrom(string address)
    {
        var addressParts = address.Split('@');
        if (addressParts.Length < 2 || addressParts[0].Trim().Length == 0 || addressParts[1].Trim().Length == 0)
        { 
            return null;
        }

        return new ValidEmailAddress(address);
    }
}
