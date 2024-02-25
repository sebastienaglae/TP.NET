using BookLibrary.Client.OpenApi.Model;
using Riok.Mapperly.Abstractions;

namespace BookLibrary.Client.Models;

[Mapper]
public static partial class Mapper
{
    public static partial Author ToAuthor(this AuthorDto dto);
    public static partial Genre ToGenre(this GenreDto dto);
    public static partial Book ToBook(this BookDto dto);
    public static partial Book ToBook(this BookListEntryDto dto);
    public static partial List<Genre> ToGenres(this List<GenreDto> dto);
    public static partial List<Author> ToAuthors(this List<AuthorDto> dto);
}