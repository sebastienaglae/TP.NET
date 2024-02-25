namespace BookLibrary.Server.Dtos;

public record struct BookDto(
    int Id,
    string Title,
    BookAuthorDto[] Authors,
    string Description,
    string[] Pages,
    decimal Price,
    BookGenreDto[] Genres
);

public record struct BookGenreDto(
    int Id,
    string Name
);

public record struct BookAuthorDto(
    int Id,
    string FirstName,
    string LastName
);

public record struct BookListEntryDto(
    int Id,
    string Title,
    BookAuthorDto[] Authors,
    decimal Price,
    BookGenreDto[] Genres
);