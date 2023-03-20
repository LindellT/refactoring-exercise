using Domain;

namespace ApplicationServices;

public sealed record UpdateUserCommand
{
    public const string ValidationRequirements = "Either email or password has to be valid.";

    private UpdateUserCommand(int id, ValidEmailAddress? emailAddress, ValidPassword? password)
    {
        Id = id;
        EmailAddress = emailAddress;
        Password = password;
    }


    public int Id { get; private set; }

    public ValidEmailAddress? EmailAddress { get; init; }

    public ValidPassword? Password { get; init; }

    public static UpdateUserCommand? CreateFrom(int id, ValidEmailAddress? emailAddress, ValidPassword? password)
    {
        if (emailAddress is null && password is null)
        {
            return null;
        }

        return new UpdateUserCommand(id, emailAddress, password);
    }
}