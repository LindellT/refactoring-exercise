using Domain;
using OneOf;

namespace ApplicationServices;

public sealed record UpdateUserCommand
{
    private UpdateUserCommand(int id, ValidEmailAddress? emailAddress, ValidPassword? password)
    {
        Id = id;
        EmailAddress = emailAddress;
        Password = password;
    }


    public int Id { get; init; }

    public ValidEmailAddress? EmailAddress { get; init; }

    public ValidPassword? Password { get; init; }

    public static OneOf<UpdateUserCommand, UpdateUserCommandValidationError> CreateFrom(int id, ValidEmailAddress? emailAddress, ValidPassword? password)
    {
        if (emailAddress is null && password is null)
        {
            return new UpdateUserCommandValidationError();
        }

        return new UpdateUserCommand(id, emailAddress, password);
    }
}