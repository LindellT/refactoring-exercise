namespace Domain;

public sealed record User(int Id, ValidEmailAddress Email, HashedPassword HashedPassword);