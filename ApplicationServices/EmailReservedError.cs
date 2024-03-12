namespace ApplicationServices;

#pragma warning disable CA1032 // Implement standard exception constructors, this exception isn't intended for general use
#pragma warning disable CA1710 // Identifiers should have correct suffix, exception used only as return type currently
public sealed class EmailReservedError : Exception
#pragma warning restore CA1710 // Identifiers should have correct suffix
#pragma warning restore CA1032 // Implement standard exception constructors
{
    public EmailReservedError() : base("Email reserved.")
    {
    }
}