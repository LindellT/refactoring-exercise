namespace Domain;

public record User(int Id, ValidEmailAddress Email, HashedPassword HashedPassword);