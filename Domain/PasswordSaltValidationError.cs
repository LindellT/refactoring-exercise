namespace Domain
{
#pragma warning disable CA1032 // Implement standard exception constructors, this exception isn't intended for general use
#pragma warning disable CA1710 // Identifiers should have correct suffix, exception used only as return type currently
    public sealed class PasswordSaltValidationError : Exception
#pragma warning restore CA1710 // Identifiers should have correct suffix
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public PasswordSaltValidationError() : base("Invalid salt. Salt length must be at least 32 characters.")
        {
        }
    }
}