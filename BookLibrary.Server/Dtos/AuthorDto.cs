namespace BookLibrary.Server.Dtos;

public record struct AuthorDto(
    int Id,
    string FirstName,
    string LastName
);