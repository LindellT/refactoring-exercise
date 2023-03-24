namespace Domain
{
    public sealed class PasswordSaltValidationError : Exception
    {
        public PasswordSaltValidationError() : base("Invalid salt. Salt length must be at least 32 characters.")
        {
        }
    }
}
