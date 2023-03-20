using Domain;

namespace ApplicationServices;

public sealed record CreateUserCommand(ValidEmailAddress EmailAddress, ValidPassword Password);